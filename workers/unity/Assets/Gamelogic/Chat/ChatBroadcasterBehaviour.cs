using UnityEngine;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Chat;

namespace Assets.Gamelogic.Chat
{
    [EngineType(EnginePlatform.FSim)]
    public class ChatBroadcasterBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            //Todo: Handle sent chat messages here!
        }

        private void OnDisable()
        {
	        //Todo: Tidy up after chat implementation here!
        }
    }
}
