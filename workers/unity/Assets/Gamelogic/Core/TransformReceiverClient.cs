using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class TransformReceiverClient : MonoBehaviour
    {
        [Require]
        private TransformComponent.Reader transformComponent;

        private Rigidbody myRigidbody;
        private bool isRemote;

        private void OnEnable()
        {
            transformComponent.ComponentUpdated += OnTransformComponentUpdated;
            myRigidbody = GetComponent<Rigidbody>();
            if (IsNotAnAuthoritativePlayer())
            {
                SetUpRemoteTransform();
            }     
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated -= OnTransformComponentUpdated;
            if (isRemote)
            {
                TearDownRemoveTransform();
            }
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
            myRigidbody.velocity = Vector3.zero;
            myRigidbody.MovePosition(position);
        }

        private bool IsNotAnAuthoritativePlayer()
        {
            return !gameObject.HasAuthority(ClientAuthorityCheck.ComponentId);
        }

        private void Update()
        {
            if (IsNotAnAuthoritativePlayer())
            {
                myRigidbody.MovePosition(Vector3.Lerp(myRigidbody.position, transformComponent.Data.position.ToVector3(), 0.2f));
                myRigidbody.MoveRotation(Quaternion.Euler(0f, ComponentUtils.DequantizeAngle(transformComponent.Data.rotation), 0f));
            }
            else if(isRemote)
            {
                TearDownRemoveTransform();
            }
        }

        private void SetUpRemoteTransform()
        {
            isRemote = true;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = true;
        }

        private void TearDownRemoveTransform()
        {
            isRemote = false;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            myRigidbody.isKinematic = false;
        }
    }
}
