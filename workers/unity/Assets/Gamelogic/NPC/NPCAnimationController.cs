using Assets.Gamelogic.Core;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [EngineType(EnginePlatform.Client)]
    public class NPCAnimationController : MonoBehaviour
    {
        [Require] private NPCLumberjack.Reader lumberjack;
        [Require] private TargetNavigation.Reader targetNavigation;

        private AudioSource audioSource;
        private Animator anim;

        [SerializeField] private ParticleSystem WoodChipsAnimation;
        [SerializeField] private GameObject HandSocket;
        [SerializeField] private GameObject Axe;
        [SerializeField] private GameObject Log;
        [SerializeField] private AudioClip[] ChoppingSounds;

        private void OnEnable()
        {
            InitialiseAudioSource();
            anim = GetComponent<Animator>();
            lumberjack.ComponentUpdated += OnLumberjackComponentUpdate;
            targetNavigation.ComponentUpdated += NavigationUpdated;
            SetAnimationState(lumberjack.Data.currentState);
            SetForwardSpeed(targetNavigation.Data.hasTarget);
        }

        private void OnDisable()
        {
            lumberjack.ComponentUpdated -= OnLumberjackComponentUpdate;
            targetNavigation.ComponentUpdated -= NavigationUpdated;
        }

        public void OnAxeConnect()
        {
            WoodChipsAnimation.Play();
            PlayRandomSound(ChoppingSounds);
        }

        private void InitialiseAudioSource()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.volume = SimulationSettings.NPCChopVolume;
            audioSource.spatialBlend = 1;
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
                anim.SetFloat("ForwardSpeed", 1);
            }
            else
            {
                anim.SetFloat("ForwardSpeed", 0);
            }
        }

        private void OnLumberjackComponentUpdate(NPCLumberjack.Update newState)
        {
            SetAnimationState(newState.currentState.Value);
        }

        private void SetAnimationState(LumberjackFSMState.StateEnum currentState)
        {
            StopCurrentAnimation();
            switch (currentState)
            {
                case LumberjackFSMState.StateEnum.STOCKPILING:
                    anim.SetBool("Dropping", true);
                    break;
                case LumberjackFSMState.StateEnum.HARVESTING:
                    anim.SetBool("Chopping", true);
                    Axe.SetActive(true);
                    HandSocket.GetComponent<FixedJoint>().connectedBody = Axe.GetComponent<Rigidbody>();
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_STOCKPILE:
                    anim.SetBool("Carrying", true);
                    Log.SetActive(true);
                    HandSocket.GetComponent<FixedJoint>().connectedBody = Log.GetComponent<Rigidbody>();
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_TREE:
                    anim.SetBool("Walking", true);
                    break;
            }
        }

        private void StopCurrentAnimation()
        {
            anim.SetBool("Dropping", false);
            anim.SetBool("Chopping", false);
            anim.SetBool("Carrying", false);
            anim.SetBool("Walking", false);
            Axe.SetActive(false);
            Log.SetActive(false);
        }

        private void PlayRandomSound(AudioClip[] choppingSounds)
        {
            int soundToPlay = new System.Random().Next(choppingSounds.Length);
            audioSource.PlayOneShot(choppingSounds[soundToPlay]);
        }
    }
}
