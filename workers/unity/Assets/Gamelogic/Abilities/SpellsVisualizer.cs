using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.UI;
using Improbable.Abilities;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Abilities
{
    [EngineType(EnginePlatform.Client)]
    public class SpellsVisualizer : MonoBehaviour
    {
        [Require] private Spells.Reader spells;

        private void OnEnable()
        {
            spells.ComponentUpdated += OnComponentUpdated;
        }

        private void OnDisable()
        {
            spells.ComponentUpdated -= OnComponentUpdated;
        }

        private void OnComponentUpdated(Spells.Update update)
        {
            for (int i = 0; i < update.spellAnimationEvent.Count; i++)
            {
                PlaySpellEffect(update.spellAnimationEvent[i].spellType, update.spellAnimationEvent[i].position.ToVector3());
            }
        }
        
        private void PlaySpellEffect(SpellType spellType, Vector3 position)
        {
            var visual = SpellsVisualizerPool.GetSpellObject(spellType);
            visual.transform.position = position;

            if (spellType.Equals(SpellType.RAIN))
            {
                visual.transform.position = position + Vector3.up * SimulationSettings.RainCloudSpawnHeight;
            }

            visual.SetActive(true);
            StartCoroutine(DeactiveSelfDelayed(visual, spellType, SimulationSettings.SpellEffectDuration));
        }

        private IEnumerator DeactiveSelfDelayed(GameObject visual, SpellType spellType, float duration)
        {
            yield return new WaitForSeconds(duration);

            visual.SetActive(false);
            SpellsVisualizerPool.ReturnSpellObject(spellType, visual);
        }
    }
}
