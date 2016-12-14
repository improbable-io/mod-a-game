using Improbable.Fire;
using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.HQ
{
    public class HQHealthBehaviour : MonoBehaviour
    {
        [Require] private Health.Reader health;
        [Require] private Flammable.Writer flammable;

        private void OnEnable()
        {
            health.ComponentUpdated += OnHealthUpdated;
        }

        private void OnDisable()
        {
            health.ComponentUpdated -= OnHealthUpdated;
        }

        private void OnHealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue)
            {
                UpdateHQFlammablility(update.currentHealth.Value);
            }
        }

        private void UpdateHQFlammablility(int health)
        {
            if (health <= 0)
            {
                flammable.Send(new Flammable.Update().SetIsOnFire(false).SetCanBeIgnited(false));
            }
            else if (!flammable.Data.canBeIgnited)
            {
                flammable.Send(new Flammable.Update().SetCanBeIgnited(true));
            }
        }
    }
}
