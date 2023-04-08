using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    [RequireComponent(typeof(Image), typeof(CanvasRenderer))]
    public class BackgroundTextureOffset : MonoBehaviour
    {
        [SerializeField] private Vector2 speedOffset;
        [SerializeField] private Image image;
        
        private static readonly int MainTex = Shader.PropertyToID("_MainTex");

        private void Start()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {
            Vector2 materialOffset = image.materialForRendering.GetTextureOffset(MainTex);
            materialOffset.x += Time.deltaTime * speedOffset.x;
            materialOffset.y += Time.deltaTime * speedOffset.y;
            image.materialForRendering.SetTextureOffset(MainTex, materialOffset);
        }

        private void OnValidate()
        {
            image = GetComponent<Image>();
        }
    }
}