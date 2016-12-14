using Assets.Gamelogic.Core;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class PlayerControlsSender : MonoBehaviour
    {
        [Require] private PlayerControls.Writer playerControls;

        private TransformReceiverClientControllableAuthoritative transformReceiverClientControllableAuthoritative;
        private Rigidbody playerRigidbody;
        private Vector3 movementDirection = Vector3.zero;     

        private void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            transformReceiverClientControllableAuthoritative = GetComponent<TransformReceiverClientControllableAuthoritative>();
        }

        private void FixedUpdate()
        {
            UpdatePlayerControls();
        }

        public void SetInputDirection(Vector3 inputDirection)
        {
            movementDirection = Vector3.ClampMagnitude((Camera.main.transform.rotation * inputDirection).FlattenVector(), 1f);
            transformReceiverClientControllableAuthoritative.SetTargetVelocity(movementDirection);
        }

        private void UpdatePlayerControls()
        { 
            var targetPosition = playerRigidbody.position;
            if (ShouldUpdatePlayerControls(targetPosition))
            {
                playerControls.Send(new PlayerControls.Update().SetTargetPosition(targetPosition.ToCoordinates()));
            }
        }

        private bool ShouldUpdatePlayerControls(Vector3 newPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newPosition, playerControls.Data.targetPosition.ToVector3());
        }
    }
}
