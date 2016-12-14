using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [EngineType(EnginePlatform.Client)]
    public class NPCWizardAnimationController : MonoBehaviour {

        [Require] private NPCWizard.Reader npcWizard;
        [Require] private TargetNavigation.Reader targetNavigation;

        private Animator Anim;
        public ParticleSystem CastAnim;

        private void Awake()
        {
            Anim = GetComponentInChildren<Animator>();
            Anim.enabled = true;
        }

        private void OnEnable()
        {
            npcWizard.ComponentUpdated += StateUpdated;
            targetNavigation.ComponentUpdated += NavigationUpdated;

            SetAnimationState(npcWizard.Data.currentState);
            SetForwardSpeed(targetNavigation.Data.hasTarget);
        }

        private void OnDisable()
        {
            npcWizard.ComponentUpdated -= StateUpdated;
            targetNavigation.ComponentUpdated -= NavigationUpdated;
        }

        private void StateUpdated(NPCWizard.Update stateUpdate)
        {
            if (stateUpdate.currentState.HasValue)
            {
                SetAnimationState(stateUpdate.currentState.Value);
            }
        }

        private void NavigationUpdated(TargetNavigation.Update navigationUpdate)
        {
            if (navigationUpdate.hasTarget.HasValue)
            {
                SetForwardSpeed(navigationUpdate.hasTarget.Value);
            }
        }

        private void SetForwardSpeed(bool hasTarget)
        {
            if (hasTarget)
            {
                Anim.SetFloat("ForwardSpeed", 1);
            }
            else
            {
                Anim.SetFloat("ForwardSpeed", 0);
            }
        }


        private void SetAnimationState(WizardFSMState.StateEnum currentState)
        {
            if (currentState.Equals(WizardFSMState.StateEnum.ATTACKING_TARGET) ||
                currentState.Equals(WizardFSMState.StateEnum.DEFENDING_TARGET))
            {
                Anim.SetTrigger("Casting");
                CastAnim.Play();
            }
            else
            {
                CastAnim.Stop();
            }
        }
    }
}
