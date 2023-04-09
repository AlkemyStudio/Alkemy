using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VoxelDeflagrationController : MonoBehaviour
{
    // Start is called before the first frame update
    private VisualEffect visualEffect;
    private bool waitForEnd = false;
    private bool particlesSpawned = false;
    void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();
    }

    public void Play()
    {
        visualEffect.Play();
        waitForEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (waitForEnd && particlesSpawned && visualEffect.aliveParticleCount <= 500)
        {
            Destroy(gameObject);
        }

        if (!particlesSpawned && visualEffect.aliveParticleCount >= 500)
        {
            particlesSpawned = true;
        }


    }
}
