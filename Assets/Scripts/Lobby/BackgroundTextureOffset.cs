using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    [RequireComponent(typeof(Image), typeof(CanvasRenderer))]
    public class BackgroundTextureOffset : MonoBehaviour
    {
        [SerializeField] private Vector2 speedOffset;
        private Vector2 materialOffset;
        [SerializeField] private Image image;
        private CanvasRenderer canvasRenderer;
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private void Start()
        {
            canvasRenderer = GetComponent<CanvasRenderer>();
            image = GetComponent<Image>();
            materialOffset = image.material.mainTextureOffset;
        }

        private void Update()
        {
            Material material = canvasRenderer.GetMaterial();
            if (material == null) return;
            
            materialOffset.x += Time.deltaTime * speedOffset.x;
            materialOffset.y += Time.deltaTime * speedOffset.y;

            canvasRenderer.GetMaterial().SetTextureOffset(MainTex, materialOffset);
        }

        private void OnValidate()
        {
            image = GetComponent<Image>();
        }
    }
}