using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VoxelGraph : MonoBehaviour
{
    public VoxelParser voxelParser;
    public bool autoDestroy = true;
    public int destroyThreshold = 100;
    private Texture3D texture3D;
    private VisualEffect visualEffect;

    private bool playing = false;
    private bool particleSpawned = false;

    public delegate void AnimationEnd();
    public event AnimationEnd OnAnimationEnd;

    void OnEnable() {
        visualEffect = GetComponent<VisualEffect>();
        if (voxelParser.voxelData.IsReady()) {
            generateTexture3D(voxelParser.voxelData);
        } else {
            voxelParser.voxelData.OnVoxelDataReady += generateTexture3D;
        }
    }

    void generateTexture3D(VoxelData voxelData) {
        texture3D = new Texture3D(voxelData.width, voxelData.height, voxelData.depth, TextureFormat.RGBA32, false);
        texture3D.filterMode = FilterMode.Point;
        texture3D.wrapMode = TextureWrapMode.Clamp;
        texture3D.SetPixelData(voxelData.voxels, 0, 0);
        texture3D.Apply();

        visualEffect.SetTexture("Voxel", texture3D);
        visualEffect.SetVector3("Size", new Vector3(voxelData.width, voxelData.height, voxelData.depth));
        visualEffect.SetVector3("Voxel Scale", voxelData.scale);
        visualEffect.SetVector3("Voxel Origin", voxelData.origin);
        
        visualEffect.Play();
        playing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (autoDestroy) {
            if (playing && particleSpawned && visualEffect.aliveParticleCount <= destroyThreshold) {
                OnAnimationEnd?.Invoke();
                Destroy(gameObject);
            }

            if (!particleSpawned && visualEffect.aliveParticleCount >= destroyThreshold) {
                particleSpawned = true;
            }
        }
    }
}
