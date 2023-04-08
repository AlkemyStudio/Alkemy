using UnityEngine;

namespace Player
{
    public class PlayerModelAnimation : MonoBehaviour
    {

        private float movementDelta = 0;
        [SerializeField] private float movementSpeed = 0.2f;
        private float actualMovementSpeed = 0.2f;
        [SerializeField] private float movementThreshold = 0.1f;


        // Start is called before the first frame update
        void Start()
        {
            actualMovementSpeed = movementSpeed;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            movementDelta += Time.deltaTime * actualMovementSpeed;

            if ((actualMovementSpeed > 0 && movementDelta > movementThreshold) || (actualMovementSpeed < 0 && movementDelta < -movementThreshold))
            {
                actualMovementSpeed = -actualMovementSpeed;
            }

            transform.Translate(new Vector3(0, Time.deltaTime * actualMovementSpeed, 0));
        }

        private void OnValidate()
        {
            actualMovementSpeed = actualMovementSpeed > 0 ? movementSpeed : -movementSpeed;
        }
    }
}
