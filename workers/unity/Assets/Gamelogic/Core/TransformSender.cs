using Improbable.Core;
using Improbable.Math;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.FSim)]
    public class TransformSender : MonoBehaviour
    {
        [Require] private TransformComponentWriter transformComponent;

        private int fixedFramesSinceLastUpdate = 0;

        public void TriggerTeleport(Vector3 position)
        {
            transform.position = position;
            transformComponent.Update.Position(position.ToCoordinates()).TriggerTeleportEvent(position.ToCoordinates()).FinishAndSend();
        }

        private void OnEnable()
        {
            transform.position = transformComponent.Position.ToVector3();
        }

        private void FixedUpdate()
        {
            var newPosition = transform.position.ToCoordinates();
            var newRotation = ComponentUtils.QuantizeAngle(transform.rotation.eulerAngles.y);
            fixedFramesSinceLastUpdate++;
            if ((PositionNeedsUpdate(newPosition) || RotationNeedsUpdate(newRotation)) && fixedFramesSinceLastUpdate > SimulationSettings.TransformUpdatesToSkipBetweenSends)
            {
                fixedFramesSinceLastUpdate = 0;
                transformComponent.Update.Position(newPosition).Rotation(newRotation).FinishAndSend();    
            }
        }

        private bool PositionNeedsUpdate(Coordinates newPosition)
        {
            return !MathUtils.CompareEqualityEpsilon(newPosition, transformComponent.Position);
        }

        private bool RotationNeedsUpdate(float newRotation)
        {
            return !MathUtils.CompareEqualityEpsilon(newRotation, transformComponent.Rotation);
        }
    }
}
