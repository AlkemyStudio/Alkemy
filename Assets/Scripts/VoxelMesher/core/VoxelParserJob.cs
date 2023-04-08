using System;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;

struct VoxelInfo {
    public short x;
    public short y;
    public short z;
    public byte red;
    public byte green;
    public byte blue;
    public byte alpha;
}

public struct PlyVoxelParseJob : IJob {
    [ReadOnly]
    public NativeArray<char> fileData;

    public NativeArray<JobVoxelData> voxelData;
    public NativeList<int> voxels;

    public NativeList<int> voxelsShape;
    public NativeList<int> voxelsCoordinates;
    public NativeList<int> voxelsColors;

    public void Execute() {
        string text = new string(fileData.ToArray());
        JobVoxelData voxelData = new JobVoxelData();
        voxelData.canExplode = false;
        voxelData.scale = Vector3.one;

        bool endHeader = false;
        List<string> properties = new List<string>();
        List<VoxelInfo> voxelsInfo = new List<VoxelInfo>();

        int minX = int.MaxValue;
        int maxX = int.MinValue;
        int minY = int.MaxValue;
        int maxY = int.MinValue;
        int minZ = int.MaxValue;
        int maxZ = int.MinValue;

        var splitLines = text.IndexOf("\r\n", StringComparison.Ordinal) > -1 
            ? text.Split("\r\n") 
            : text.Split('\n');
        
        foreach (string line in splitLines) {
            if (line == "end_header") {
                endHeader = true;
                continue;
            }

            if (!endHeader) {
                string[] parts = line.Split(' ');
                switch (parts[0]) {
                    case "property":
                        properties.Add(parts[2]);
                        break;
                    case "explode":
                        voxelData.canExplode = true;
                        break;
                    case "origin":
                        voxelData.origin = new Vector3(float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    case "scale":
                        voxelData.scale = new Vector3(float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parts[3], System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    default:
                        break;
                }
            } else if (line.Length > 0) {
                string[] parts = line.Split(' ');
                VoxelInfo voxelInfo = new VoxelInfo();
                voxelInfo.alpha = 255;
                for (int i = 0; i < parts.Length; i++) {
                    switch (properties[i]) {
                        case "x":
                            voxelInfo.x = short.Parse(parts[i], System.Globalization.CultureInfo.InvariantCulture);
                            minX = Mathf.Min(minX, voxelInfo.x);
                            maxX = Mathf.Max(maxX, voxelInfo.x);
                            break;
                        case "y":
                            voxelInfo.y = short.Parse(parts[i], System.Globalization.CultureInfo.InvariantCulture);
                            minY = Mathf.Min(minY, voxelInfo.y);
                            maxY = Mathf.Max(maxY, voxelInfo.y);
                            break;
                        case "z":
                            voxelInfo.z = short.Parse(parts[i], System.Globalization.CultureInfo.InvariantCulture);
                            minZ = Mathf.Min(minZ, voxelInfo.z);
                            maxZ = Mathf.Max(maxZ, voxelInfo.z);
                            break;
                        case "red":
                            voxelInfo.red = byte.Parse(parts[i]);
                            break;
                        case "green":
                            voxelInfo.green = byte.Parse(parts[i]);
                            break;
                        case "blue":
                            voxelInfo.blue = byte.Parse(parts[i]);
                            break;
                        case "alpha":
                            voxelInfo.alpha = byte.Parse(parts[i]);
                            break;
                    }
                }
                voxelsInfo.Add(voxelInfo);
            }
        }
        // Compute the size of the voxel grid
        voxelData.width = maxX - minX + 1;
        voxelData.height = maxY - minY + 1;
        voxelData.depth = maxZ - minZ + 1;

        // Resize the capacity of the voxel array to avoid reallocations
        voxels.Resize(voxelData.width * voxelData.height * voxelData.depth, NativeArrayOptions.ClearMemory);
        voxelsShape.Resize(voxelData.width * voxelData.height * voxelData.depth, NativeArrayOptions.ClearMemory);

        for (int i = 0; i < voxelsInfo.Count; i++) {
            VoxelInfo voxelInfo = voxelsInfo[i];
            int index = (voxelInfo.x - minX) + (voxelInfo.y - minY) * voxelData.width + (voxelInfo.z - minZ) * voxelData.width * voxelData.height;
            // Encode the color in the voxel
            voxels[index] = (voxelInfo.red) | (voxelInfo.green << 8) | (voxelInfo.blue << 16) | (voxelInfo.alpha << 24);
        }

        int voxelShapeCount = 0;
        // Compute the shape of the voxel grid
        for (int i = 0; i < voxelsInfo.Count; i++) {
            VoxelInfo voxelInfo = voxelsInfo[i];
            int index = (voxelInfo.x - minX) + (voxelInfo.y - minY) * voxelData.width + (voxelInfo.z - minZ) * voxelData.width * voxelData.height;

            if (IsTransparent(voxels, voxelInfo.x - minX - 1, voxelInfo.y - minY, voxelInfo.z - minZ, voxelData.width, voxelData.height, voxelData.depth) ||
                IsTransparent(voxels, voxelInfo.x - minX + 1, voxelInfo.y - minY, voxelInfo.z - minZ, voxelData.width, voxelData.height, voxelData.depth) ||
                IsTransparent(voxels, voxelInfo.x - minX, voxelInfo.y - minY - 1, voxelInfo.z - minZ, voxelData.width, voxelData.height, voxelData.depth) ||
                IsTransparent(voxels, voxelInfo.x - minX, voxelInfo.y - minY + 1, voxelInfo.z - minZ, voxelData.width, voxelData.height, voxelData.depth) ||
                IsTransparent(voxels, voxelInfo.x - minX, voxelInfo.y - minY, voxelInfo.z - minZ - 1, voxelData.width, voxelData.height, voxelData.depth) ||
                IsTransparent(voxels, voxelInfo.x - minX, voxelInfo.y - minY, voxelInfo.z - minZ + 1, voxelData.width, voxelData.height, voxelData.depth)
                ) {
                    voxelsShape[index] = (voxelInfo.red) | (voxelInfo.green << 8) | (voxelInfo.blue << 16) | (voxelInfo.alpha << 24);
                    voxelShapeCount++;
                }
        }

        voxelsCoordinates.Resize(voxelShapeCount, NativeArrayOptions.UninitializedMemory);
        voxelsColors.Resize(voxelShapeCount, NativeArrayOptions.UninitializedMemory);

        int voxelCoordinateIndex = 0;
        for (int x = 0; x < voxelData.width; x++) {
            for (int y = 0; y < voxelData.height; y++) {
                for (int z = 0; z < voxelData.depth; z++) {
                    int index = x + y * voxelData.width + z * voxelData.width * voxelData.height;
                    if (voxelsShape[index] != 0) {
                        int voxelIndex = voxelCoordinateIndex++;
                        voxelsCoordinates[voxelIndex] = (x & 0xFF) | ((y & 0xFF) << 8) | ((z & 0xFF) << 16);
                        voxelsColors[voxelIndex] = voxelsShape[index];
                    }
                }
            }
        }


        this.voxelData[0] = voxelData;
    }

    private bool IsTransparent(NativeList<int> voxels, int x, int y, int z, int width, int height, int depth) {
        if (x < 0 || x >= width || y < 0 || y >= height || z < 0 || z >= depth) {
            return true;
        }

        int index = x + y * width + z * width * height;
        int color = voxels[index];

        return ((color >> 24) & 0xFF) == 0;
    }
}