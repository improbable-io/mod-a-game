using System.Collections;
using Improbable.Life;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Visualizes characters in the world dying with an explosion of smoke
    /// </summary>
    [EngineType(EnginePlatform.Client)]
    public class CharacterDeathVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;

        private void OnEnable()
        {
            health.ComponentUpdated += HealthUpdated;
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= HealthUpdated;
        }

        private void HealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0)
            {
                PlayDeathEffect();
            }
        }

        private void PlayDeathEffect()
        {
            var visual = DeathAnimVisualizerPool.GetEffectObject();
            visual.transform.position = transform.position + Vector3.up * SimulationSettings.DeathEffectSpawnHeight;
            visual.SetActive(true);
            StartCoroutine(DeactiveSelfDelayed(visual, visual.GetComponent<ParticleSystem>().duration));
        }

        private IEnumerator DeactiveSelfDelayed(GameObject visual, float duration)
        {
            yield return new WaitForSeconds(duration);
            visual.SetActive(false);
            DeathAnimVisualizerPool.ReturnEffectObject(visual);
        }
    }
}
