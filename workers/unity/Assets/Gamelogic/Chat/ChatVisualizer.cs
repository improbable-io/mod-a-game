using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Chat
{
    [EngineType(EnginePlatform.Client)]
    public class ChatVisualizer : MonoBehaviour
    {
        [Require] private Improbable.Chat.Chat.Reader chat;

        private NotificationController notification;

        private void Awake()
        {
            notification = GetComponentInChildren<NotificationController>();
            if (notification == null)
            {
                Debug.LogWarning("No notification controller!");
            }
            else
            {
                notification.HideNotification();
            }
        }

        private void OnEnable()
        {
            chat.ComponentUpdated += ComponentUpdated;
        }

        private void OnDisable()
        {
            chat.ComponentUpdated -= ComponentUpdated;

            if (notification != null)
            {
                notification.HideNotification();
            }
        }

        private void ComponentUpdated(Improbable.Chat.Chat.Update update)
        {
            var lastIndex = update.chatSent.Count - 1;
            notification.ShowNotification(update.chatSent[lastIndex].message);
        }
    }
}
