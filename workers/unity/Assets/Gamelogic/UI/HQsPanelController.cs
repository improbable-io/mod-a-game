using Assets.Gamelogic.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Gamelogic.UI
{
    public class HQsPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject hqIconPrefab;

        private static IList<GameObject> teamHqToIcons = new List<GameObject>();
        private static uint localPlayerTeamId = 0;

        private void Awake()
        {
            PopulateHqIcons();
        }

        public static void SetLocalPlayerTeamId(uint teamId)
        {
            localPlayerTeamId = teamId;
            AlignHqIcons();
        }

        public static void SetHQHealth(uint teamId, float health)
        {
            var hqIcon = teamHqToIcons[(int)teamId];
            var hqController = hqIcon.GetComponent<HQIconController>();

            var normalizedHealth = health / SimulationSettings.HQMaxHealth;
            hqController.SetHQHealth(normalizedHealth);
            AlignHqIcons();
        }

        private static void SetupPlayerHqIcon(GameObject hqIcon)
        {
            var hqIconRectTransform = hqIcon.GetComponent<RectTransform>();
            hqIconRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            hqIconRectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.anchorMax = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.pivot = new Vector2(0.0f, 1.0f);
            hqIconRectTransform.anchoredPosition = Vector3.zero;
        }

        private static void SetupOpponentTeamsHqIcons(GameObject hqIcon, int iconIndex)
        {
            var hqIconRectTransform = hqIcon.GetComponent<RectTransform>();
            var scaleAdjustment = 0.897f;
            hqIconRectTransform.anchoredPosition = new Vector3(-15.0f + (-iconIndex * hqIconRectTransform.rect.width * hqIconRectTransform.localScale.x * scaleAdjustment), -11.0f, 0.0f);
        }

        private static void SetupSingleOpponentTeamHqIcon(GameObject hqIcon)
        {
            var hqIconRectTransform = hqIcon.GetComponent<RectTransform>();
            hqIconRectTransform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
            hqIconRectTransform.anchoredPosition = new Vector3(-6.0f, -5.0f, 0.0f);
        }

        private void PopulateHqIcons()
        {
            for (var index = 0; index < SimulationSettings.TeamCount; index++)
            {
                var rectTransform = GetComponent<RectTransform>();
                var hqIcon = (GameObject) Instantiate(hqIconPrefab, rectTransform);
                teamHqToIcons.Add(hqIcon);
            }

            AlignHqIcons();
        }

        private static void AlignHqIcons()
        {
            var teamCount = SimulationSettings.TeamCount;

            var opponentTeamIndex = 0;
            for (var index = 0; index < teamCount; index++)
            {
                var hqIcon = teamHqToIcons[index];

                var hqController = hqIcon.GetComponent<HQIconController>();
                hqController.SetTeamId(index);

                if (index == localPlayerTeamId)
                {
                    SetupPlayerHqIcon(hqIcon);
                }
                else
                {
                    if (teamCount == 2)
                    {
                        SetupSingleOpponentTeamHqIcon(hqIcon);
                    }
                    else
                    {
                        SetupOpponentTeamsHqIcons(hqIcon, opponentTeamIndex);
                        opponentTeamIndex++;
                    }
                }
            }
        }
    }
}