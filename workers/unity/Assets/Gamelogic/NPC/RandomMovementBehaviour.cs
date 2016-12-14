using Assets.Gamelogic.Core;
using Improbable.Npc;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Random = System.Random;

namespace Assets.Gamelogic.NPC
{
    public class RandomMovementBehaviour : MonoBehaviour
    {
        [Require] private TargetNavigation.Reader targetNavigation;

        private TargetNavigationBehaviour navigation;
        private Random rand = new Random();

        private void OnEnable ()
        {
            navigation = GetComponent<TargetNavigationBehaviour>();
            targetNavigation.ComponentUpdated += NavigationUpdated;
        }

        private void OnDisable()
        {
            targetNavigation.ComponentUpdated -= NavigationUpdated;
        }

        private void OnTriggerEnter(Collider _)
        {
            if (targetNavigation != null)
            {
                SelectNewTarget();
            }
        }

        private void NavigationUpdated(TargetNavigation.Update navigationUpdate)
        {
            if (targetNavigation.HasAuthority && !targetNavigation.Data.hasTarget)
            {
                SelectNewTarget();
            }
        }

        private void SelectNewTarget()
        {
            Vector3 CurrentPosition = transform.position;
            Vector3 NewTargetPosition = CalculateTargetPosition(CurrentPosition);
            navigation.SetNavigation(NewTargetPosition.ToCoordinates());
        }

        private Vector3 CalculateTargetPosition(Vector3 CurrentPosition)
        {
            Quaternion NewTargetRelativeRotation = Quaternion.Euler(0, (float)rand.NextDouble() * 360, 0);
            Vector3 NewTargetOffset = (NewTargetRelativeRotation * transform.forward) * SimulationSettings.NPCTargetSettingDistance;
            Vector3 NewTargetPosition = CurrentPosition + NewTargetOffset;
            return NewTargetPosition;
        }
    }
}
