using Improbable.Core;
using Improbable.Fire;
using Improbable.Npc;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [EngineType(EnginePlatform.FSim)]
    public class NPCLumberjackOnFireBehaviour : MonoBehaviour
    {
        [Require] private NPCLumberjack.Writer npcState;
        [Require] private FSimAuthorityCheck.Writer fSimAuthorityCheck;
        [Require] private Flammable.Reader flammable;

        private NPCLumberjackBehaviour lumberjackBehaviour;

        private void OnEnable()
        {
            lumberjackBehaviour = GetComponent<NPCLumberjackBehaviour>();
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
                    lumberjackBehaviour.ChangeStateTo(LumberjackFSMState.StateEnum.ON_FIRE);
                    break;
                case false:
                    lumberjackBehaviour.ChangeStateTo(LumberjackFSMState.StateEnum.IDLE);
                    break;
            }
        }
    }
}
