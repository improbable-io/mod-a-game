using Assets.Gamelogic.Core;
using Improbable.Fire;
using Improbable.Unity.Visualizer;
using System.Collections;
using Assets.Gamelogic.Life;
using Improbable.Core;
using UnityEngine;

namespace Assets.Gamelogic.Fire
{
    class FireHealthInteractionBehaviour : MonoBehaviour
    {
        [Require] private FSimAuthorityCheck.Writer fSimAuthorityCheck;
        [Require] private Flammable.Reader flammable;

        private HealthBehaviour healthBehaviour;
        private Coroutine takeDamageFromFireCoroutine;

        private void OnEnable()
        {
            healthBehaviour = GetComponent<HealthBehaviour>();
            takeDamageFromFireCoroutine = StartCoroutine(TakeDamagefromFireCoroutine(SimulationSettings.SimulationTickInterval));
        }

        private void OnDisable()
        {
            CancelTakeDamagefromFireCoroutine();
        }

        private void CancelTakeDamagefromFireCoroutine()
        {
            if (takeDamageFromFireCoroutine != null)
            {
                StopCoroutine(takeDamageFromFireCoroutine);
                takeDamageFromFireCoroutine = null;
            }
        }

        private IEnumerator TakeDamagefromFireCoroutine(float interval)
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                if (flammable.Data.isOnFire)
                {
                    healthBehaviour.AddCurrentHealthDelta(-SimulationSettings.FireDamagePerTick);
                }
            }
        }
    }
}
