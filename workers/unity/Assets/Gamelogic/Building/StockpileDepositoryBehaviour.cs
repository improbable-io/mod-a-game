using Assets.Gamelogic.Life;
using Improbable.Building;
using Improbable.Core;
using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class StockpileDepositoryBehaviour : MonoBehaviour
    {
        [Require] private StockpileDepository.Writer stockpileDepository;
        [Require] private Health.Writer health; // Required for healthBehaviour
        private HealthBehaviour healthBehaviour;

        private void OnEnable ()
        {
            healthBehaviour = GetComponent<HealthBehaviour>();
            stockpileDepository.CommandReceiver.OnAddResource += OnAddResource;
        }

        private void OnDisable()
        {
            stockpileDepository.CommandReceiver.OnAddResource -= OnAddResource;
        }

        private void OnAddResource(Improbable.Entity.Component.ResponseHandle<StockpileDepository.Commands.AddResource, AddResource, Improbable.Core.Nothing> request)
        {
            if (stockpileDepository.Data.canAcceptResources)
            {
                healthBehaviour.AddCurrentHealthDelta(request.Request.quantity);
            }
            request.Respond(new Nothing());
        }
    }
}
