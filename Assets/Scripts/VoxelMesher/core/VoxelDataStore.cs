using Unity.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelDataStore {

    private static VoxelDataStore instance;

    private Dictionary<string, VoxelData> store = new Dictionary<string, VoxelData>();

    public static VoxelDataStore Instance {
        get {
            if (instance == null) {
                instance = new VoxelDataStore();
                Application.quitting += () => {
                    instance.Dispose();
                };
            }
            return instance;
        }
    }

    public VoxelData SetVoxelData(string instanceID, VoxelData voxelData) {
        store.TryAdd(instanceID, voxelData);
        return store[instanceID];
    }

    public VoxelData GetVoxelData(string instanceID) {
        if (store.ContainsKey(instanceID)) {
            return store[instanceID];
        }
        return null;
    }

    public void Dispose() {
        foreach (var voxelData in store.Values) {
            voxelData.Dispose();
        }
    }
}