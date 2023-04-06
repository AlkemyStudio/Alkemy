using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using VoxelMesher.core;

namespace VoxelMesher
{
    public class MultiVoxelParser : MonoBehaviour
    {
        public TextAsset[] files;
        public VoxelData[] VoxelDatas { get; private set; }
        [HideInInspector] public JobHandle[] ParserJobHandlers;

        private bool parserRunning = false;
        private PlyVoxelParseJob[] parseJobs;

        private void Start()
        {
            VoxelDatas = new VoxelData[files.Length];
            parseJobs = new PlyVoxelParseJob[files.Length];
            ParserJobHandlers = new JobHandle[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                TextAsset file = files[i];
                
                if (file == null) continue;
                
                VoxelDatas[i] = VoxelDataStore.GetVoxelData(file.name);

                if (VoxelDatas[i] == null && file.text.StartsWith("ply"))
                {
                    VoxelData newVoxelData = new VoxelData();
                    newVoxelData.name = file.name;
                    VoxelDatas[i] = newVoxelData;

                    VoxelDataStore.SetVoxelData(file.name, VoxelDatas[i]);
                    PlyVoxelParseJob newVoxelParserJob = new PlyVoxelParseJob
                    {
                        fileData = new NativeArray<char>(file.text.ToCharArray(), Allocator.Persistent),
                        voxelData = new NativeArray<JobVoxelData>(1, Allocator.Persistent),
                        voxels = new NativeList<int>(Allocator.Persistent)
                    };
                    ParserJobHandlers[i] = newVoxelParserJob.Schedule();
                    parseJobs[i] = newVoxelParserJob;
                    parserRunning = true;
                }
            }
        }

        public string[] GetFileNames()
        {
            return files.Select(file => file.name).ToArray();
        }
        
        private void LateUpdate()
        {
            bool shouldStop = true;
            
            for (int i = 0; i < ParserJobHandlers.Length; i++)
            {
                JobHandle parserJobHandler = ParserJobHandlers[i];
                
                if (parserJobHandler.IsCompleted)
                {
                    parserJobHandler.Complete();

                    VoxelData voxelData = VoxelDatas[i];
                    PlyVoxelParseJob parseJob = parseJobs[i];
                    
                    if (VoxelDatas == null) continue;
                    
                    voxelData.canExplode = parseJob.voxelData[0].canExplode;
                    voxelData.origin = parseJob.voxelData[0].origin;
                    voxelData.scale = parseJob.voxelData[0].scale;
                    voxelData.width = parseJob.voxelData[0].width;
                    voxelData.height = parseJob.voxelData[0].height;
                    voxelData.depth = parseJob.voxelData[0].depth;
                    voxelData.voxels = parseJob.voxels.ToArray();
                    
                    parseJob.fileData.Dispose();
                    parseJob.voxelData.Dispose();
                    parseJob.voxels.Dispose();

                    voxelData.MarkReady();
                    VoxelDatas[i] = voxelData;
                }
                else
                {
                    shouldStop = false;
                }
            }

            if (shouldStop)
            {
                enabled = false;
            }
        }
    }
}