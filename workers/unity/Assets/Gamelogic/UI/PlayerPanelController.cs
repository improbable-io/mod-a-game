using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class PlayerPanelController : MonoBehaviour
    {
        public static PlayerPanelController Instance { private set; get; }

        [SerializeField] private Image playerHealthIcon;
        
        private void Awake()
        {
            Instance = this;
        }

        public static void SetPlayerHealth(float fill)
        {
            Instance.playerHealthIcon.fillAmount = fill;
        }
    }
}
