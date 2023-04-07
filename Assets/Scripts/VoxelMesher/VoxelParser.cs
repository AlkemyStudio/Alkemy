using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;


public class VoxelParser : MonoBehaviour
{
    public TextAsset file;

    public VoxelData voxelData;

    [System.NonSerialized]
    public JobHandle parserJobHandle;
    private PlyVoxelParseJob parseJob;
    private bool isParsing = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (file == null) return;

        voxelData = VoxelDataStore.GetVoxelData(file.name);

        if (voxelData == null) {
            if (file.text.StartsWith("ply")) {
                voxelData = new VoxelData();
                voxelData.name = file.name;

                VoxelDataStore.SetVoxelData(file.name, voxelData);
                parseJob = new PlyVoxelParseJob();
                parseJob.fileData = new NativeArray<char>(file.text.ToCharArray(), Allocator.Persistent);
                parseJob.voxelData = new NativeArray<JobVoxelData>(1, Allocator.Persistent);
                parseJob.voxels = new NativeList<int>(Allocator.Persistent);
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

        parseJob.fileData.Dispose();
        parseJob.voxelData.Dispose();
        parseJob.voxels.Dispose();

        voxelData.MarkReady();
            
        // Stop the script
        enabled = false;
    }
}
