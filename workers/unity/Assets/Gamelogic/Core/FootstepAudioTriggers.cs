using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class FootstepAudioTriggers : MonoBehaviour
    {
        [SerializeField] private AudioClip LeftFootStep;
        [SerializeField] private AudioClip RightFootStep;

        private AudioSource audioSource;

        private void OnEnable()
        {
            InitialiseAudioSource();
        }

        private void InitialiseAudioSource()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.volume = SimulationSettings.FootstepVolume;
        }

        public void OnLeftFootStep()
        {
            audioSource.PlayOneShot(LeftFootStep);
        }

        public void OnRightFootStep()
        {
            audioSource.PlayOneShot(RightFootStep);
        }
    }
}
