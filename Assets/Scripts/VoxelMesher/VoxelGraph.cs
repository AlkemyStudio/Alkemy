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
    private VisualEffect visualEffect;

    private bool playing = false;
    private bool particleSpawned = false;

    public delegate void AnimationEnd();
    public event AnimationEnd OnAnimationEnd;

    public void Awake() {
        visualEffect = GetComponent<VisualEffect>();
    }

    void OnEnable() {
        if (voxelParser.voxelData.IsReady()) {
            generateTexture3D(voxelParser.voxelData);
        } else {
            voxelParser.voxelData.OnVoxelDataReady += generateTexture3D;
        }
    }

    void generateTexture3D(VoxelData voxelData) {
        visualEffect.SetVector3("Size", new Vector3(voxelData.width, voxelData.height, voxelData.depth));
        visualEffect.SetVector3("Voxel Scale", voxelData.scale);
        visualEffect.SetVector3("Voxel Origin", voxelData.origin);
        visualEffect.SetInt("Voxel Count", voxelData.voxelsCoordinates.Length);
        visualEffect.SetGraphicsBuffer("Voxel Colors", voxelData.voxelsColorBuffer);
        visualEffect.SetGraphicsBuffer("Voxel Coordinates", voxelData.voxelsCoordinateBuffer);
        
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
