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

        // This code scrolls the MainTex texture of the material assigned to the image component.
        // The image component is assigned to the variable image.
        // The speedOffset variable stores the speed at which the texture should be scrolled.
        // The MainTex variable stores the name of the texture that should be scrolled.

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