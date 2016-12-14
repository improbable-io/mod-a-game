using System.Collections;
using Assets.Gamelogic.Core;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Life;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class PlayerInfoBehaviour : MonoBehaviour
    {
        [Require] private PlayerInfo.Writer playerInfo;
        [Require] private Health.Writer health;
        [Require] private Flammable.Writer flammable;
        [Require] private Spells.Writer spells;
        [Require] private Inventory.Writer inventory;

        private TransformSender transformSender;

        private void Awake()
        {
            transformSender = GetComponent<TransformSender>();
        }

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
                DieUponHealthDepletion(update);
            }
        }

        private void DieUponHealthDepletion(Health.Update update)
        {
            if (update.currentHealth.Value <= 0)
            {
                Die();
                StartCoroutine(RespawnDelayed(SimulationSettings.PlayerRespawnDelay));
            }
        }

        private void Die()
        {
            playerInfo.Send(new PlayerInfo.Update().SetIsAlive(false));
            health.Send(new Health.Update().SetCanBeChanged(false));
            flammable.Send(new Flammable.Update().SetIsOnFire(false).SetCanBeIgnited(false));
            spells.Send(new Spells.Update().SetCooldowns(new Map<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } }).SetCanCastSpells(false));
        }

        private IEnumerator RespawnDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);
            Respawn();
        }

        private void Respawn()
        {
            transformSender.TriggerTeleport(playerInfo.Data.initialSpawnPosition.ToVector3());
            health.Send(new Health.Update().SetCurrentHealth(SimulationSettings.PlayerMaxHealth).SetCanBeChanged(true));
            flammable.Send(new Flammable.Update().SetCanBeIgnited(true));
            spells.Send(new Spells.Update().SetCanCastSpells(true));
            inventory.Send(new Inventory.Update().SetResources(0));
            playerInfo.Send(new PlayerInfo.Update().SetIsAlive(true));
        }
    }
}
