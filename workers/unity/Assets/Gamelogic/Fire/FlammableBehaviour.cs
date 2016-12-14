using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Fire;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable;
using Improbable.Unity.Core;
using System.Collections;
using Assets.Gamelogic.Core;
using Improbable.Unity;

namespace Assets.Gamelogic.Fire
{
    [EngineType(EnginePlatform.FSim)]
    class FlammableBehaviour : MonoBehaviour
    {
        [Require] private Flammable.Writer flammable;

        public bool IsOnFire
        {
            get { return flammable != null && flammable.Data.isOnFire; }
        }

        private FlammableBehaviourOptimize optimize;
        private Collider[] nearbyColliders = new Collider[8];
        private Coroutine spreadFlamesCoroutine;

        private void Awake()
        {
            optimize = GetComponent<FlammableBehaviourOptimize>();
        }

        private void OnEnable()
        {
            flammable.CommandReceiver.OnExtinguish += OnExtinguish;
            flammable.CommandReceiver.OnIgnite += OnIgnite;

            if (flammable.Data.isOnFire)
            {
                StartFlameSpread();
            }
        }

        private void OnDisable()
        {
            flammable.CommandReceiver.OnExtinguish -= OnExtinguish;
            flammable.CommandReceiver.OnIgnite -= OnIgnite;

            StopFlameSpread();
        }

        private void OnIgnite(ResponseHandle<Flammable.Commands.Ignite, Nothing, Nothing> request)
        {
            Ignite();
            request.Respond(new Nothing());
        }

        private void OnExtinguish(ResponseHandle<Flammable.Commands.Extinguish, ExtinguishRequest, Nothing> request)
        {
            Extinguish(request.Request.canBeIgnited);
            request.Respond(new Nothing());
        }

        private void Ignite()
        {
            if (!flammable.Data.isOnFire && flammable.Data.canBeIgnited)
            {
                optimize.IgniteUpdate();
                StartFlameSpread();
            }
        }

        private void Extinguish(bool canBeIgnited)
        {
            if (flammable.Data.isOnFire)
            {
                optimize.ExtinguishUpdate(canBeIgnited);
                StopFlameSpread();
            }
        }

        public void SelfIgnite(IComponentWriter writer)
        {
            if (flammable == null)
            {
                SpatialOS.Commands.SendCommand(writer, Flammable.Commands.Ignite.Descriptor, new Nothing(), 
                    gameObject.EntityId(), _ => { });
                return;
            }

            Ignite();
        }

        public void SelfExtinguish(IComponentWriter writer, bool canBeIgnited)
        {
            if (flammable == null)
            {
                SpatialOS.Commands.SendCommand(writer, Flammable.Commands.Extinguish.Descriptor, new ExtinguishRequest(canBeIgnited), 
                    gameObject.EntityId(), _ => { });
                return;
            }

            Extinguish(canBeIgnited);
        }

        private void StartFlameSpread()
        {
            spreadFlamesCoroutine = StartCoroutine(SpreadFlamesLoop());
        }

        private void StopFlameSpread()
        {
            if (spreadFlamesCoroutine != null)
            {
                StopCoroutine(spreadFlamesCoroutine);
            }
        }

        private IEnumerator SpreadFlamesLoop()
        {
            yield return new WaitForSeconds(SimulationSettings.FireSpreadInterval);

            if (flammable == null)
            {
                yield break;
            }

            int count = Physics.OverlapSphereNonAlloc(transform.position, SimulationSettings.FireSpreadRadius, nearbyColliders);
            for (int i = 0; i < count; i++)
            {
                var otherFlammable = nearbyColliders[i].transform.GetComponentInParent<FlammableDataVisualizer>();
                if (otherFlammable == null || otherFlammable.flammable == null || !otherFlammable.flammable.Data.canBeIgnited)
                {
                    continue;
                }

                otherFlammable.GetComponent<FlammableBehaviour>().SelfIgnite(flammable);
            }
        }
    }
}
