using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class HorizontalRotation : MonoBehaviour
    {
        public bool ClockWiseRotation = true;
        [SerializeField] private float RotationSpeed = 80.0f;

        void Update()
        {
            var rotationDirection = (ClockWiseRotation ? 1f : -1f);
            transform.Rotate(rotationDirection * Vector3.up * RotationSpeed * Time.deltaTime);
        }
    }
}
