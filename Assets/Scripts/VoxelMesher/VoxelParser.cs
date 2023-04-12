using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Serialization;

namespace VoxelMesher
{
    public class VoxelParser : MonoBehaviour
    {
        public TextAsset file;
        public VoxelData voxelData;
        
        [SerializeField] private VoxelMeshFilter voxelMeshFilter;

        [HideInInspector]
        public JobHandle parserJobHandle;
        private PlyVoxelParseJob parseJob;
        private bool isParsing = false;

        // Start is called before the first frame update
        private void Start()
        {
            // UnsafeUtility.SetLeakDetectionMode(NativeLeakDetectionMode.EnabledWithStackTrace);
            if (file == null) return;

            voxelData = VoxelDataStore.Instance.GetVoxelData(file.name);

            if (voxelData == null) {
                if (file.text.StartsWith("ply")) {
                    voxelData = new VoxelData();
                    voxelData.name = file.name;

                    VoxelDataStore.Instance.SetVoxelData(file.name, voxelData);
                    parseJob = new PlyVoxelParseJob();
                    parseJob.fileData = new NativeArray<char>(file.text.ToCharArray(), Allocator.Persistent);
                    parseJob.voxelData = new NativeArray<JobVoxelData>(1, Allocator.Persistent);
                    parseJob.voxels = new NativeList<int>(Allocator.Persistent);
                    parseJob.voxelsShape = new NativeList<int>(Allocator.Persistent);
                    parseJob.voxelsCoordinates = new NativeList<int>(Allocator.Persistent);
                    parseJob.voxelsColors = new NativeList<int>(Allocator.Persistent);
                    parserJobHandle = parseJob.Schedule();
                
                    isParsing = true;
                }
            }
        }

        private void LateUpdate()
        {
            if (!isParsing || !parserJobHandle.IsCompleted) return;
        
            parserJobHandle.Complete();
            isParsing = false;

            voxelData.canExplode = parseJob.voxelData[0].canExplode;
            voxelData.origin = parseJob.voxelData[0].origin;
            voxelData.scale = parseJob.voxelData[0].scale;
            voxelData.width = parseJob.voxelData[0].width;
            voxelData.height = parseJob.voxelData[0].height;
            voxelData.depth = parseJob.voxelData[0].depth;
            voxelData.voxels = parseJob.voxels.ToArray();
            voxelData.voxelsColors = parseJob.voxelsColors.ToArray();
            voxelData.voxelsCoordinates = parseJob.voxelsCoordinates.ToArray();

            voxelData.voxelsColorBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, voxelData.voxelsColors.Length, 4);
            voxelData.voxelsColorBuffer.SetData(voxelData.voxelsColors);

            voxelData.voxelsCoordinateBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, voxelData.voxelsCoordinates.Length, 4);
            voxelData.voxelsCoordinateBuffer.SetData(voxelData.voxelsCoordinates);

            parseJob.fileData.Dispose();
            parseJob.voxelData.Dispose();
            parseJob.voxels.Dispose();
            parseJob.voxelsShape.Dispose();
            parseJob.voxelsCoordinates.Dispose();
            parseJob.voxelsColors.Dispose();

            voxelData.MarkReady();
            
            // Stop the script
            enabled = false;
        }

        private void OnValidate()
        {
            if (voxelMeshFilter == null) voxelMeshFilter = GetComponent<VoxelMeshFilter>();
        }
    }
}
