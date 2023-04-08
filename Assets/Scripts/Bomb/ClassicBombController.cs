using UnityEngine;

namespace Bomb
{
    public class ClassicBombController : BaseBombController
    {
        [SerializeField] private AnimationCurve flickeringCurve;
        [SerializeField] private float baseFlickeringSpeed;
        [SerializeField] private float finalFlickeringSpeed;

        private static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");

        private void Update()
        {
            float bombLifeTime = Mathf.Min(Time.time - spawnTime, BombData.FuseTime) / BombData.FuseTime;
            float curveValue = flickeringCurve.Evaluate(bombLifeTime);
            float flickerValue = Mathf.Lerp(baseFlickeringSpeed, finalFlickeringSpeed, curveValue);

            meshRenderer.sharedMaterial.SetFloat(
                FlickerSpeed,
                flickerValue
            );
        }
    }
}