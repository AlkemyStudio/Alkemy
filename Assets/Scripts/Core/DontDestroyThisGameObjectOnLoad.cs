using System;
using UnityEngine;

namespace Core
{
    public class DontDestroyThisGameObjectOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}