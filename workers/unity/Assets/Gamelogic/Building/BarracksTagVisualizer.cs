using System.Collections.Generic;
using Assets.Gamelogic.Core;
using Improbable.Building;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    [EngineType(EnginePlatform.FSim)]
    public class BarracksTagVisualizer : MonoBehaviour {

        [Require] private BarracksInfo.Reader barracksInfo;

        private static readonly IDictionary<BarracksState, string> barracksStateToTag = new Dictionary<BarracksState, string>
        {
            { BarracksState.UNDER_CONSTRUCTION, SimulationSettings.StockpileTag },
            { BarracksState.CONSTRUCTION_FINISHED, SimulationSettings.BarracksTag }
        };

        private void OnEnable()
        {
            SetBarracksTag(barracksInfo.Data.barracksState);
            barracksInfo.ComponentUpdated += OnBarracksInfoUpdate;
        }

        private void OnDisable()
        {
            barracksInfo.ComponentUpdated -= OnBarracksInfoUpdate;
        }

        private void OnBarracksInfoUpdate(BarracksInfo.Update update)
        {
            if (update.barracksState.HasValue)
            {
                SetBarracksTag(update.barracksState.Value);
            }
        }

        private void SetBarracksTag(BarracksState barracksState)
        {
            gameObject.tag = barracksStateToTag[barracksState];
        }
    }
}
