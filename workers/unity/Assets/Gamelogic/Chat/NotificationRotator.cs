using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Chat
{
    [EngineType(EnginePlatform.Client)]
    public class NotificationRotator : MonoBehaviour
    {
        private void LateUpdate()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                transform.forward = Camera.main.transform.forward;
            }
        }
    }
}