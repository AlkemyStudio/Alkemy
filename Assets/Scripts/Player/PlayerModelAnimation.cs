using UnityEngine;

namespace Player
{
    /// <summary>
    /// PlayerModelAnimation is used to animate the player model.
    /// </summary>
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

        /// <summary>
        /// Move the player during the fixed update to keep accurate physics detection.
        /// </summary>
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
