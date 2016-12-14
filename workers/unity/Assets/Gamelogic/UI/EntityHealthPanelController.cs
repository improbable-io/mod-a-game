using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class EntityHealthPanelController : MonoBehaviour
    {
        private GameObject entityHealthPanel;
        private Image entityHealthIcon;

        private void Awake()
        {
            entityHealthPanel = transform.FindChild("EntityHealthPanel").gameObject;
            entityHealthIcon = transform.FindChild("EntityHealthPanel").FindChild("ActiveImage").GetComponent<Image>();
        }

        public void Show()
        {
            entityHealthPanel.SetActive(true);
        }

        public void Hide()
        {
            entityHealthPanel.SetActive(false);
        }

        public void SetHealth(float fill)
        {
            entityHealthIcon.fillAmount = fill;
        }
    }
}
