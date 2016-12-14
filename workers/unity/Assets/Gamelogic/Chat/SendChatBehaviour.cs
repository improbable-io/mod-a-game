using Improbable;
using Improbable.Core;
using UnityEngine;
using Improbable.Chat;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Chat
{
    [EngineType(EnginePlatform.Client)]
    public class SendChatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;
        
        public void SayChat(string message)
        {
			//Todo: Send a chat message here!
        }
    }
}