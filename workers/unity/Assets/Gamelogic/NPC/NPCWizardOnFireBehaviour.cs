using Improbable.Core;
using Improbable.Fire;
using Improbable.Npc;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    public class NPCWizardOnFireBehaviour : MonoBehaviour
    {
        [Require] private FSimAuthorityCheck.Writer fSimAuthorityCheck;
        [Require] private Flammable.Reader flammable;
        [Require] private NPCWizard.Writer wizardWriter;

        private NPCWizardBehaviour npcWizardBehaviourInterface;
        
        private void Awake()
        {
            npcWizardBehaviourInterface = GetComponent<NPCWizardBehaviour>();
        }

        private void OnEnable()
        {
            flammable.ComponentUpdated += OnFlammableUpdated;
        }

        private void OnDisable()
        {
            flammable.ComponentUpdated -= OnFlammableUpdated;
        }

        private void OnFlammableUpdated(Flammable.Update update)
        {
            if (update.isOnFire.HasValue)
            {
                UpdateStateMachine();
            }
        }

        private void UpdateStateMachine()
        {
            switch (flammable.Data.isOnFire)
            {
                case true:
                    npcWizardBehaviourInterface.ChangeTo(WizardFSMState.StateEnum.ON_FIRE);
                    break;
                case false:
                    npcWizardBehaviourInterface.ChangeTo(WizardFSMState.StateEnum.IDLE);
                    break;
            }
        }
    }
}
