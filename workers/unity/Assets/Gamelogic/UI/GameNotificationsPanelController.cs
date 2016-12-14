using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class GameNotificationsPanelController : MonoBehaviour
    {
        private static Text text;
        
        private void Awake()
        {
            text = transform.FindChild("Text").GetComponent<Text>();
        }

        public static void SetText(string t)
        {
            text.text = t;
        }
    }
}
