using UnityEngine;

namespace Core
{
    public class InfiniteRotation : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 1f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;

        private void FixedUpdate()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
