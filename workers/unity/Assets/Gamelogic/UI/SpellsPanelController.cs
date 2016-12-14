using Assets.Gamelogic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class SpellsPanelController : MonoBehaviour
    {
        private static Image lightningCircleIcon;
        private static Image rainCircleIcon;
        private static Image lightningSpellIcon;
        private static Image rainSpellIcon;
        
        private void Awake()
        {
            lightningCircleIcon = transform.FindChild("LightningIcon").FindChild("ActiveImage").GetComponent<Image>();
            rainCircleIcon = transform.FindChild("RainIcon").FindChild("ActiveImage").GetComponent<Image>();
            lightningSpellIcon = transform.FindChild("LightningIcon").FindChild("LightningImage").GetComponent<Image>();
            rainSpellIcon = transform.FindChild("RainIcon").FindChild("RainImage").GetComponent<Image>();
        }

        public static void SetLightningIconFill(float fill)
        {
            lightningCircleIcon.fillAmount = fill;
            lightningSpellIcon.color = fill < 1f ? SimulationSettings.TransparentWhite : Color.white;
        }

        public static void SetRainIconFill(float fill)
        {
            rainCircleIcon.fillAmount = fill;
            rainSpellIcon.color = fill < 1f ? SimulationSettings.TransparentWhite : Color.white;
        }
    }
}
