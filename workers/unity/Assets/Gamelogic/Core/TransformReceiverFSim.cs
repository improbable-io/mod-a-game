using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.FSim)]
    public class TransformReceiverFSim : MonoBehaviour
    {
        [Require] private TransformComponent.Reader transformComponent;
        private Rigidbody myRigidbody;

        private void OnEnable()
        {
            myRigidbody = GetComponent<Rigidbody>();
            UpdateTransform();
            transformComponent.ComponentUpdated += OnComponentUpdated;
        }

        private void OnDisable()
        {
            transformComponent.ComponentUpdated -= OnComponentUpdated;
        }

        private void OnComponentUpdated(TransformComponent.Update update)
        {
            if (!transformComponent.HasAuthority)
            {
                UpdateTransform();
            }
        }

        private void UpdateTransform()
        {
            myRigidbody.MovePosition(transformComponent.Data.position.ToVector3());
            myRigidbody.MoveRotation(Quaternion.Euler(0f, ComponentUtils.DequantizeAngle(transformComponent.Data.rotation), 0f));
        }        
    }
}
