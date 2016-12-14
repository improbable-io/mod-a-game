using UnityEngine;
using UnityEngine.UI;

namespace Assets.Gamelogic.UI
{
    public class HQIconController : MonoBehaviour
    {
        [SerializeField] private Image hqIcon;
        [SerializeField] private Color[] teamMaterials;

        public void SetTeamId(int teamId)
        {
            hqIcon.color = teamMaterials[teamId];
        }

        public void SetHQHealth(float fill)
        {
            hqIcon.fillAmount = fill;
        }
    }
}

