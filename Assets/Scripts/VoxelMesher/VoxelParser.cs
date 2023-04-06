using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VoxelMesher.core;

namespace VoxelMesher
{
    public class VoxelParser : MonoBehaviour
    {
        public TextAsset file;

        public VoxelData VoxelData;

        [HideInInspector]
        public JobHandle ParserJobHandle;

        private bool parserRunning = false;
        private PlyVoxelParseJob parseJob;

        // Start is called before the first frame update
        private void Start()
        {
            if (file == null) return;

            VoxelData = VoxelDataStore.GetVoxelData(file.name);

            if (VoxelData == null) {
                if (file.text.StartsWith("ply")) {
                    VoxelData = new VoxelData();
                    VoxelData.name = file.name;

                    VoxelDataStore.SetVoxelData(file.name, VoxelData);
                    parseJob = new PlyVoxelParseJob();
                    parseJob.fileData = new NativeArray<char>(file.text.ToCharArray(), Allocator.Persistent);
                    parseJob.voxelData = new NativeArray<JobVoxelData>(1, Allocator.Persistent);
                    parseJob.voxels = new NativeList<int>(Allocator.Persistent);
                    ParserJobHandle = parseJob.Schedule();
                    parserRunning = true;
                }
            }
        }

        private void LateUpdate()
        {
            if (!parserRunning || !ParserJobHandle.IsCompleted) return;
        
            ParserJobHandle.Complete();
            parserRunning = false;
            enabled = false;

            VoxelData.canExplode = parseJob.voxelData[0].canExplode;
            VoxelData.origin = parseJob.voxelData[0].origin;
            VoxelData.scale = parseJob.voxelData[0].scale;
            VoxelData.width = parseJob.voxelData[0].width;
            VoxelData.height = parseJob.voxelData[0].height;
            VoxelData.depth = parseJob.voxelData[0].depth;
            VoxelData.voxels = parseJob.voxels.ToArray();

            parseJob.fileData.Dispose();
            parseJob.voxelData.Dispose();
            parseJob.voxels.Dispose();

            VoxelData.MarkReady();
        }
    }
}
