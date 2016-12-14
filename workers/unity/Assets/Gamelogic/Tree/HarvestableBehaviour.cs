using Assets.Gamelogic.Core;
using Improbable.Entity.Component;
using Improbable.Tree;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class HarvestableBehaviour : MonoBehaviour
    {
        [Require] private Harvestable.Writer harvestable;

        [SerializeField] private int remainingResources;

        private void OnEnable()
        {
            harvestable.CommandReceiver.OnYieldHarvest += OnYieldHarvest;
        }

        private void OnDisable()
        {
            harvestable.CommandReceiver.OnYieldHarvest -= OnYieldHarvest;
        }

        private void OnYieldHarvest(ResponseHandle<Harvestable.Commands.YieldHarvest, YieldHarvestRequest, HarvestResponse> request)
        {
            var resourcesToGive = Mathf.Min(SimulationSettings.HarvestReturnQuantity, remainingResources);
            
            remainingResources = Mathf.Max(harvestable.Data.resources - resourcesToGive, 0);

            harvestable.Send(new Harvestable.Update().SetResources(remainingResources));
            request.Respond(new HarvestResponse(resourcesToGive));
        }

        public void ResetResourceCount()
        {
            harvestable.Send(new Harvestable.Update().SetResources(SimulationSettings.InitialResourceTotal));
        }
    }
}
