using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

public enum FaceDirection {
    UP,
    DOWN,
    LEFT,
    RIGHT,
    FRONT,
    BACK
}

public enum MeshingAlgorithm {
    NAIVE = 1,
    GREEDY = 2
}

public struct VoxelMeshJob : IJob {
    [ReadOnly]
    public NativeArray<int> voxels;

    [ReadOnly]
    public Vector3 origin;

    [ReadOnly]
    public int width;

    [ReadOnly]
    public int height;

    [ReadOnly]
    public int depth;

    [ReadOnly]
    public MeshingAlgorithm algorithm;

    public NativeList<Vector3> vertices;
    public NativeList<int> triangles;
    public NativeList<Color32> colors;

    public void Execute() {
        if (algorithm == MeshingAlgorithm.NAIVE) {
            NaiveMeshing();
        } else if (algorithm == MeshingAlgorithm.GREEDY) {
            
        }
    }

    void NaiveMeshing() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {
                    int color = voxels[GetIndex(x, y, z)];
                    if ((color & 0xFF) == 0) continue;

                    if (IsVoxelEmpty(x, y + 1, z)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.UP);
                    if (IsVoxelEmpty(x, y - 1, z)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.DOWN);
                    if (IsVoxelEmpty(x - 1, y, z)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.LEFT);
                    if (IsVoxelEmpty(x + 1, y, z)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.RIGHT);
                    if (IsVoxelEmpty(x, y, z - 1)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.BACK);
                    if (IsVoxelEmpty(x, y, z + 1)) PushFace(x - origin.x, y - origin.y, z - origin.z, color, FaceDirection.FRONT);
                }
            }
        }
    }

    bool IsVoxelEmpty(int x, int y, int z) {
        if (x < 0 || x >= width) return true;
        if (y < 0 || y >= height) return true;
        if (z < 0 || z >= depth) return true;

        if ((voxels[GetIndex(x, y, z)] & 0xFF) == 0) return true;
        return false;
    }

    private int GetIndex(int x, int y, int z) {
        return x + y * width + z * width * height;
    }

    private void PushFace(float x, float y, float z, int color, FaceDirection dir) {
        int start = vertices.Length;

        byte A = (byte)(color & 0xFF);
        byte R = (byte)((color >> 8) & 0xFF);
        byte G = (byte)((color >> 16) & 0xFF);
        byte B = (byte)((color >> 24) & 0xFF);

        switch (dir) {
            case FaceDirection.UP:
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                break;
            case FaceDirection.DOWN:
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x + 1, y, z));
                break;
            case FaceDirection.LEFT:
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x, y, z));
                vertices.Add(new Vector3(x, y, z + 1));
                break;
            case FaceDirection.RIGHT:
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z));
                break;
            case FaceDirection.BACK:
                vertices.Add(new Vector3(x + 1, y + 1, z));
                vertices.Add(new Vector3(x, y + 1, z));
                vertices.Add(new Vector3(x + 1, y, z));
                vertices.Add(new Vector3(x, y, z));
                break;
            case FaceDirection.FRONT:
                vertices.Add(new Vector3(x, y + 1, z + 1));
                vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                vertices.Add(new Vector3(x, y, z + 1));
                vertices.Add(new Vector3(x + 1, y, z + 1));
                break;
            default:
                break;
        }

        colors.Add(new Color32(R, G, B, A));
        colors.Add(new Color32(R, G, B, A));
        colors.Add(new Color32(R, G, B, A));
        colors.Add(new Color32(R, G, B, A));

        triangles.Add(start);
        triangles.Add(start + 2);
        triangles.Add(start + 1);
        triangles.Add(start + 2);
        triangles.Add(start + 3);
        triangles.Add(start + 1);
    }
}