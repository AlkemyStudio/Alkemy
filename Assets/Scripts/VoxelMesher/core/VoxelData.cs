using UnityEngine;
using UnityEngine.VFX;



public class VoxelData {
    public string name;
    public int[] voxels;
    public int[] voxelsColors;
    public int[] voxelsCoordinates;
    public int width;
    public int height;
    public int depth;
    public Vector3 origin;
    public Vector3 scale;
    public bool canExplode;

    public GraphicsBuffer voxelsColorBuffer;
    public GraphicsBuffer voxelsCoordinateBuffer;

    private bool _ready = false;

    public delegate void VoxelDataReady(VoxelData self);
    public event VoxelDataReady OnVoxelDataReady;

    public void MarkReady()
    {
        if (_ready) return;
        _ready = true;
        OnVoxelDataReady?.Invoke(this);
    }

    public bool IsReady() {
        return _ready;
    }

    ~VoxelData() {
        voxelsColorBuffer.Dispose();
        voxelsCoordinateBuffer.Dispose();
    }
}

public struct JobVoxelData {
    public int width;
    public int height;
    public int depth;
    public Vector3 origin;
    public Vector3 scale;
    public bool canExplode;
}