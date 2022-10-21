using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VoxelMeshFilter : MonoBehaviour
{
    private MeshFilter meshFilter;
    private VoxelParser parser;
    private VoxelMesh voxelMesh;
    private VoxelMeshJob meshJob;
    private JobHandle meshJobHandle;
    private bool meshJobRunning = false;
    void Start()
    {
        parser = GetComponent<VoxelParser>();

        if (parser == null || parser.voxelData == null) return;

        if (parser.voxelData.IsReady()) {
            setupVoxelMesh(parser.voxelData);
        } else {
            parser.voxelData.OnVoxelDataReady += setupVoxelMesh;
        }
    }

    private void setupVoxelMesh(VoxelData voxelData) {
        voxelMesh = VoxelMeshStore.GetVoxelMesh(voxelData.name);
        meshFilter = GetComponent<MeshFilter>();

        if (voxelMesh != null) {
            meshFilter.mesh = voxelMesh.mesh;
        } else {
            voxelMesh = VoxelMeshStore.SetVoxelMesh(voxelData.name, new VoxelMesh(1));

            meshJob = new VoxelMeshJob();
            meshJob.algorithm = MeshingAlgorithm.NAIVE;
            meshJob.voxels = new NativeArray<int>(voxelData.voxels, Allocator.Persistent);
            meshJob.origin = voxelData.origin;
            meshJob.width = voxelData.width;
            meshJob.height = voxelData.height;
            meshJob.depth = voxelData.depth;
            meshJob.vertices = new NativeList<Vector3>(Allocator.Persistent);
            meshJob.colors = new NativeList<Color32>(Allocator.Persistent);
            meshJob.triangles = new NativeList<int>(Allocator.Persistent);

            meshJobHandle = meshJob.Schedule(parser.parserJobHandle);
            meshJobRunning = true;
        }
    }

    void LateUpdate() {
        if (meshJobRunning && meshJobHandle.IsCompleted) {
            meshJobHandle.Complete();

            meshJobRunning = false;
            voxelMesh.mesh.SetVertices(meshJob.vertices.ToArray());
            voxelMesh.mesh.SetColors(meshJob.colors.ToArray());
            voxelMesh.mesh.SetTriangles(meshJob.triangles.ToArray(), 0);
            voxelMesh.mesh.Optimize();
            voxelMesh.mesh.RecalculateNormals();
            voxelMesh.mesh.RecalculateBounds();

            meshFilter.mesh = voxelMesh.mesh;

            meshJob.voxels.Dispose();
            meshJob.vertices.Dispose();
            meshJob.colors.Dispose();
            meshJob.triangles.Dispose();
        }
    }
}
