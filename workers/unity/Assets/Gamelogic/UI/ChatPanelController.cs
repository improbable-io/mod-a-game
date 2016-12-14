using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class ChatPanelController : MonoBehaviour
    {
        private static InputField chatInputField;
        private static string lastMessage = "";
        public static bool ChatModeActive;

        private void Awake()
        {
            chatInputField = transform.FindChild("InputField").GetComponent<InputField>();
        }

        private void OnEnable()
        {
            DeactivateChatMode();
        }

        private void OnDisable()
        {
            DeactivateChatMode();
        }

        private void LateUpdate()
        {
            if (chatInputField.isFocused && !ChatModeActive)
            {
                ActivateChatMode();
            }
            if (!chatInputField.isFocused && ChatModeActive)
            {
                DeactivateChatMode();
            }
        }

        public static void ActivateChatMode()
        {
            chatInputField.text = "";
            chatInputField.ActivateInputField();
            ChatModeActive = true;
        }

        public static void DeactivateChatMode()
        {
            chatInputField.text = "";
            chatInputField.DeactivateInputField();
            ChatModeActive = false;
        }

        public static void ReuseLastMessage()
        {
            chatInputField.text = lastMessage;
            chatInputField.MoveTextEnd(false);
        }

        public static string SubmitChat()
        {
            var chatMessage = chatInputField.text;
            lastMessage = chatMessage;
            DeactivateChatMode();
            return chatMessage;
        }
    }
}
