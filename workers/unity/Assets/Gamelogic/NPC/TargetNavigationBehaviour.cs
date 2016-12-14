using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Collections;
using Improbable.Fire;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    public class TargetNavigationBehaviour : MonoBehaviour
    {
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private Flammable.Reader flammable;

        private Rigidbody myRigidbody;
        private Transform myTransform;

        public void SetNavigation(Coordinates target)
        {
            targetNavigation.Send(new TargetNavigation.Update().SetHasTarget(true).SetTargetPosition(target).SetTargetEntityId(new Option<EntityId>()));
        }

        public void SetNavigation(EntityId target)
        {
            targetNavigation.Send(new TargetNavigation.Update().SetHasTarget(true).SetTargetEntityId(target).SetTargetPosition(new Option<Coordinates>()));
        }

        public void StopNavigation()
        {
            targetNavigation.Send(new TargetNavigation.Update().SetHasTarget(false));
        }

        private void Awake()
        {
            myRigidbody = GetComponent<Rigidbody>();
            myTransform = GetComponent<Transform>();
        }

        private void FixedUpdate()
        {
            if(targetNavigation.Data.hasTarget)
            {
                var targetPosition = Vector3.zero;
                if (targetNavigation.Data.targetEntityId.HasValue)
                {
                    var targetEntityId = targetNavigation.Data.targetEntityId.Value;
                    var targetEntityObject = SpatialOS.Universe.Get(targetEntityId);
                    if (targetEntityObject == null)
                    {
                        Debug.LogError("Navigation not found in universe for entity " + gameObject.EntityId());
                        FinishNavigation();
                        return;
                    }
                    targetPosition = targetEntityObject.UnderlyingGameObject.transform.position;
                }
                else
                {
                    targetPosition = targetNavigation.Data.targetPosition.Value.ToVector3();
                }
                if (Vector3.Distance(myTransform.position, targetPosition) < 3.0f)
                {
                    FinishNavigation();
                }
                else
                {
                    var movementSpeed = SimulationSettings.NPCMovementSpeed * (flammable.Data.isOnFire ? SimulationSettings.OnFireMovementSpeedIncreaseFactor : 1f);
                    myRigidbody.MovePosition(myTransform.position + (targetPosition - myTransform.position).normalized * movementSpeed * Time.fixedDeltaTime);
                    myTransform.LookAt(targetPosition, Vector3.up);
                }
            }
        }

        private void FinishNavigation()
        {
            targetNavigation.Send(new TargetNavigation.Update().SetHasTarget(false).AddReachedTarget(new ReachedTarget()));
        }
    }
}
