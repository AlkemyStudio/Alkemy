using UnityEngine;

namespace Bomb
{
    public class ClassicBombController : BaseBombController
    {
        [SerializeField] private AnimationCurve flickeringCurve;
        [SerializeField] private float baseFlickeringSpeed;
        [SerializeField] private float finalFlickeringSpeed;

        private static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");

        //This code is used to update the bomb's appearance to make it look like it is burning.
        //It uses a curve to change the speed of the flickering based on how long the bomb has been alive.
        private void Update()
        {
            float bombLifeTime = Mathf.Min(Time.time - spawnTime, fuseTime) / fuseTime;
            float curveValue = flickeringCurve.Evaluate(bombLifeTime);
            float flickerValue = Mathf.Lerp(baseFlickeringSpeed, finalFlickeringSpeed, curveValue);

            meshRenderer.sharedMaterial.SetFloat(
                FlickerSpeed,
                flickerValue
            );
        }
    }
}