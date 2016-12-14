using System.Collections;
using UnityEngine;
using Assets.Gamelogic.Core;
using Improbable.Abilities;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Player
{
    [EngineType(EnginePlatform.Client)]
    public class PlayerAnimController : MonoBehaviour
    {
        [Require] private Spells.Reader spells;

        private Vector3 lastPosition;
        private Animator Anim;
        public ParticleSystem CastAnim;

        private void OnEnable()
        {
            Anim = GetComponentInChildren<Animator>();
            Anim.enabled = true;

            lastPosition = transform.position;
        }

        private void Update()
        {
            float movementTargetDistance = (lastPosition - transform.position).magnitude;
            float animSpeed = Mathf.Min(1, movementTargetDistance / SimulationSettings.PlayerMovementTargetSlowingThreshold);
            Anim.SetFloat("ForwardSpeed", animSpeed);

            lastPosition = transform.position;
        }

        public void AnimateSpellCast()
        {
            CastAnim.Play();
            Anim.SetTrigger("CastLightning");
            StartCoroutine(CancelLightningCastAnimAfter(SimulationSettings.PlayerCastAnimationTime));
        }

        private IEnumerator CancelLightningCastAnimAfter(float playerCastAnimationTime)
        {
            yield return new WaitForSeconds(playerCastAnimationTime);
            CastAnim.Stop();
        }
    }
}
