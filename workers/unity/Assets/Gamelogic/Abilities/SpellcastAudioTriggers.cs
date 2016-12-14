using Assets.Gamelogic.Core;
using Improbable.Abilities;
using Improbable.Math;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    [EngineType(EnginePlatform.Client)]
    public class SpellcastAudioTriggers : MonoBehaviour
    {
        [Require] private Spells.Reader spells;

        private AudioSource audioSource;

        [SerializeField] private AudioClip SpellChannelAudio;
        [SerializeField] private AudioClip LightningAudio;
        [SerializeField] private AudioClip RainAudio;

        private void OnEnable ()
        {
            spells.ComponentUpdated += OnSpellCast;
            InitialiseAudioSource();
        }

        private void OnDisable()
        {
            spells.ComponentUpdated -= OnSpellCast;
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

        private void OnSpellCast(Spells.Update spellCastUpdate)
        {
            for (var eventNum = 0; eventNum < spellCastUpdate.spellAnimationEvent.Count; eventNum++)
            {
                var spellCast = spellCastUpdate.spellAnimationEvent[eventNum];
                TriggerSpellcastAudio(spellCast.spellType, spellCast.position);
            }
        }

        public void TriggerSpellChannelAudio()
        {
            audioSource.volume = SimulationSettings.SpellChannelVolume;
            audioSource.PlayOneShot(SpellChannelAudio);
        }

        private void TriggerSpellcastAudio(SpellType spellCastType, Coordinates spellCastPosition)
        {
            switch (spellCastType)
            {
                case SpellType.LIGHTNING:
                    audioSource.volume = SimulationSettings.LightningStrikeVolume;
                    audioSource.PlayOneShot(LightningAudio);
                    break;
                case SpellType.RAIN:
                    audioSource.volume = SimulationSettings.RainVolume;
                    audioSource.PlayOneShot(RainAudio);
                    break;
            }
        }
    }
}
