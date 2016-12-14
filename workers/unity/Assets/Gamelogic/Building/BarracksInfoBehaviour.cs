using Assets.Gamelogic.Core;
using Improbable.Building;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class BarracksInfoBehaviour : MonoBehaviour
    {
        [Require] private BarracksInfo.Writer barracksInfo;
        [Require] private Health.Reader health;
        [Require] private Flammable.Writer flammable;
        [Require] private TeamAssignment.Writer teamAssignment;
        [Require] private StockpileDepository.Writer stockpile; // Required for stockpileDepositoryBehaviour

        private NPCSpawnerBehaviour npcSpawnerBehaviour;

        private void Awake()
        {
            npcSpawnerBehaviour = GetComponent<NPCSpawnerBehaviour>();
        }

        private void OnEnable()
        {
            TransitionToBarracksState(barracksInfo.Data.barracksState);
            health.ComponentUpdated += OnHealthUpdated;
            RegisterWithTeamHQ();
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= OnHealthUpdated;
            UnregisterWithTeamHQ();
        }

        private void RegisterWithTeamHQ()
        {
          //  var hqEntityId = teamKnowledge.Data.teamHqEntityIds[(int)teamAssignment.Data.teamId];
          //  SpatialOS.Commands.SendCommand(barracksInfo, HQInfo.Commands.RegisterBarracks.Descriptor, new RegisterBarracksRequest(gameObject.EntityId()), hqEntityId, result => {});
        }

        private void UnregisterWithTeamHQ()
        {
           // var hqEntityId = teamKnowledge.Data.teamHqEntityIds[(int)teamAssignment.Data.teamId];
           // SpatialOS.Commands.SendCommand(barracksInfo, HQInfo.Commands.UnregisterBarracks.Descriptor, new UnregisterBarracksRequest(gameObject.EntityId()), hqEntityId, result => {});
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                EvaluateTransitionToUntouchedStockpile(update);
                EvaluateTransitionToStockpileUnderConstruction(update);
                EvaluateTransitionToBarracks(update);
            }
        }

        private void EvaluateTransitionToUntouchedStockpile(Health.Update update)
        {
            if (update.currentHealth.Value <= 0)
            {
                SpatialOS.Commands.SendCommand(flammable, Flammable.Commands.Extinguish.Descriptor, new ExtinguishRequest(false), gameObject.EntityId(), _ => { });
                TransitionToBarracksState(BarracksState.UNDER_CONSTRUCTION);
            }
        }

        private void EvaluateTransitionToStockpileUnderConstruction(Health.Update update)
        {
            if (update.currentHealth.Value > 0 && update.currentHealth.Value < SimulationSettings.BarracksMaxHealth)
            {
                flammable.Send(new Flammable.Update().SetCanBeIgnited(true));
            }
        }

        private void EvaluateTransitionToBarracks(Health.Update update)
        {
            if (barracksInfo.Data.barracksState == BarracksState.UNDER_CONSTRUCTION && update.currentHealth.Value == SimulationSettings.BarracksMaxHealth)
            {
                flammable.Send(new Flammable.Update().SetCanBeIgnited(true));
                TransitionToBarracksState(BarracksState.CONSTRUCTION_FINISHED);
            }
        }

        private void TransitionToBarracksState(BarracksState barracksState)
        {
            switch (barracksState)
            {
                case BarracksState.UNDER_CONSTRUCTION:
                    SetCanAcceptResources(true);
                    npcSpawnerBehaviour.SetSpawningEnabled(false);
                    npcSpawnerBehaviour.ResetCooldowns();
                    break;
                case BarracksState.CONSTRUCTION_FINISHED:
                    SetCanAcceptResources(false);
                    npcSpawnerBehaviour.SetSpawningEnabled(true);
                    npcSpawnerBehaviour.ResetCooldowns();
                    break;
            }
            barracksInfo.Send(new BarracksInfo.Update().SetBarracksState(barracksState));
        }

        private void SetCanAcceptResources(bool c)
        {
            if (stockpile.Data.canAcceptResources != c)
            {
                stockpile.Send(new StockpileDepository.Update().SetCanAcceptResources(c));
            }
        }
    }
}
