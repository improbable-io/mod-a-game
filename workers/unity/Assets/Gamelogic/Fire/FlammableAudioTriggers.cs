using Improbable.Fire;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    [EngineType(EnginePlatform.Client)]
    public class FlammableAudioTriggers : MonoBehaviour
    {
        [Require] private Flammable.Reader flammable;

        private AudioSource audioSource;

        [SerializeField] private AudioClip Ignite;
        [SerializeField] private AudioClip Extinguish;
        [SerializeField] private AudioClip Fire;

        private void OnEnable()
        {
            InitialiseAudioSource();
            flammable.ComponentUpdated += OnFireChange;
        }

        private void OnDisable()
        {
            flammable.ComponentUpdated -= OnFireChange;
        }

        private void InitialiseAudioSource()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.spatialBlend = 1;
        }

        private void OnFireChange(Flammable.Update fireChange)
        {
            if (fireChange.isOnFire.HasValue)
            {
                if (fireChange.isOnFire.Value)
                {
                    TriggerIgnitionSound();
                    StartFireAudio();
                }
                else
                {
                    StopFireAudio();
                    TriggerExtinguishSound();
                }
            }
        }

        public void TriggerIgnitionSound()
        {
            audioSource.volume = SimulationSettings.IgnitionVolume;
            audioSource.PlayOneShot(Ignite);
        }

        private void StartFireAudio()
        {
            audioSource.clip = Fire;
            audioSource.volume = SimulationSettings.FireVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        private void StopFireAudio()
        {
            audioSource.loop = false;
            audioSource.volume = SimulationSettings.ExtinguishVolume;
            audioSource.Stop();
        }

        public void TriggerExtinguishSound()
        {
            audioSource.PlayOneShot(Extinguish);
        }
    }
}
