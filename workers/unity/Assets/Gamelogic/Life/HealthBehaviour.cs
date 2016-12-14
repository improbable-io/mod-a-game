using Improbable.Life;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Life
{
    public class HealthBehaviour : MonoBehaviour
    {
        [Require] private Health.Writer health;

        public int CurrentHealth { get { return health.Data.currentHealth; } }
        public int MaxHealth { get { return health.Data.maxHealth; } }

        public void SetCanBeChanged(bool canBeChanged)
        {
            health.Send(new Health.Update().SetCanBeChanged(canBeChanged));
        }

        public void SetCurrentHealth(int newHealth)
        {
            if (health.Data.canBeChanged)
            {
                health.Send(new Health.Update().SetCurrentHealth(Mathf.Max(newHealth, 0)));
            }
        }

        public void AddCurrentHealthDelta(int delta)
        {
            if (health.Data.canBeChanged)
            {
                if (TryingToDecreaseHealthBelowZero(delta))
                {
                    return;
                }
                health.Send(new Health.Update().SetCurrentHealth(Mathf.Max(health.Data.currentHealth + delta, 0)));
            }
        }

        private bool TryingToDecreaseHealthBelowZero(int delta)
        {
            return health.Data.currentHealth == 0 && delta < 0;
        }
    }
}
