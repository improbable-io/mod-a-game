using Improbable.Core;
using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class TransformReceiverClientControllableAuthoritative : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private TransformComponent.Reader transformComponent;
        [Require] private Flammable.Reader flammable;

        private Rigidbody playerRigidbody;
        private Vector3 targetVelocity;

        private void Awake()
        {
            playerRigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            transformComponent.ComponentUpdated += OnTransformComponentUpdated;
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated -= OnTransformComponentUpdated;
        }

        private void OnTransformComponentUpdated(TransformComponent.Update update)
        {
            for (int i = 0; i < update.teleportEvent.Count; i++)
            {
                TeleportTo(update.teleportEvent[i].targetPosition.ToVector3());
            }
        }

        private void TeleportTo(Vector3 position)
        {
            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.MovePosition(position);
        }

        public void SetTargetVelocity(Vector3 direction)
        {
            var movementSpeed = SimulationSettings.PlayerMovementSpeed * (flammable.Data.isOnFire ? SimulationSettings.OnFireMovementSpeedIncreaseFactor : 1f);
            targetVelocity = direction * movementSpeed;
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        public void MovePlayer()
        {
            var currentVelocity = playerRigidbody.velocity;
            var velocityChange = targetVelocity - currentVelocity;
            if (ShouldMovePlayerAuthoritativeClient(velocityChange))
            {
                transform.LookAt(playerRigidbody.position + targetVelocity);
                playerRigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
            }
        }

        private bool ShouldMovePlayerAuthoritativeClient(Vector3 velocityChange)
        {
            return velocityChange.sqrMagnitude > Mathf.Epsilon && PlayerMovementCheatSafeguardPassedAuthoritativeClient(velocityChange);
        }

        private bool PlayerMovementCheatSafeguardPassedAuthoritativeClient(Vector3 velocityChange)
        {
            var result = velocityChange.sqrMagnitude < SimulationSettings.PlayerPositionUpdateMaxSqrDistance;
            if (!result)
            {
                Debug.LogError("Player movement cheat safeguard failed on Client. " + velocityChange.sqrMagnitude);
            }
            return result;
        }
    }
}
