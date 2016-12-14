using Assets.Gamelogic.UI;
using Improbable.Core;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.HQ
{
    [EngineType(EnginePlatform.Client)]
    public class HQVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;
        [Require] private TeamAssignment.Reader teamAssignment;

        private void OnEnable()
        {
            health.ComponentUpdated += OnHealthUpdated;
            UpdateHQHealthBar(teamAssignment.Data.teamId, health.Data.currentHealth);
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= OnHealthUpdated;
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                UpdateHQHealthBar(teamAssignment.Data.teamId, update.currentHealth.Value);
            }
        }

        private void UpdateHQHealthBar(uint teamId, float healthValue)
        {
            HQsPanelController.SetHQHealth(teamId, healthValue);
        }
    }
}
