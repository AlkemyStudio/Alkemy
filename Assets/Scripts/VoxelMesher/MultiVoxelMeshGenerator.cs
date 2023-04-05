// using System;
// using UnityEngine;
//
// namespace VoxelMesher
// {
//     [RequireComponent(typeof(MultiVoxelParser))]
//     public class MultiVoxelMeshGenerator : MonoBehaviour
//     {
//         private MultiVoxelParser parser;
//         
//         private void Start()
//         {
//             parser = GetComponent<MultiVoxelParser>();
//
//             if (parser == null || parser.VoxelDatas == null) return;
//             if (parser.VoxelDatas.Length <= 0) return;
//
//             foreach (VoxelData voxelData in parser.VoxelDatas)
//             {
//                 if (voxelData.IsReady())
//                 {
//                     SetupVoxelMesh(parser.voxelData);
//                 }
//                 else
//                 {
//                     parser.voxelData.OnVoxelDataReady += SetupVoxelMesh;
//                 }
//             }
//         }
//         
//         private void SetupVoxelMesh(VoxelData voxelData) {
//             voxelMesh = VoxelMeshStore.GetVoxelMesh(voxelData.name);
//             meshFilter = GetComponent<MeshFilter>();
//
//             if (voxelMesh != null) {
//                 meshFilter.mesh = voxelMesh.mesh;
//             } else {
//                 voxelMesh = VoxelMeshStore.SetVoxelMesh(voxelData.name, new VoxelMesh(1));
//                 voxelMesh.waitOptimization = true;
//
//                 meshJob = new VoxelMeshJob();
//                 meshJob.algorithm = MeshingAlgorithm.NAIVE;
//                 meshJob.voxels = new NativeArray<int>(voxelData.voxels, Allocator.Persistent);
//                 meshJob.origin = voxelData.origin;
//                 meshJob.scale = voxelData.scale;
//                 meshJob.width = voxelData.width;
//                 meshJob.height = voxelData.height;
//                 meshJob.depth = voxelData.depth;
//                 meshJob.vertices = new NativeList<Vector3>(Allocator.Persistent);
//                 meshJob.colors = new NativeList<Color32>(Allocator.Persistent);
//                 meshJob.triangles = new NativeList<int>(Allocator.Persistent);
//
//                 meshJobHandle = meshJob.Schedule(parser.parserJobHandle);
//                 meshJobRunning = true;
//             }
//         }
//     }
// }