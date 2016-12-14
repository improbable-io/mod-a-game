using System.Collections;
using System.Collections.Generic;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Player;
using Assets.Gamelogic.UI;
using Improbable.Abilities;
using Improbable.Core;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Abilities
{
    public class SpellsRequester : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer clientAuthorityCheck;
        [Require] private Spells.Reader spells;
        
        private GameObject spellAOEIndicatorInstance;
        public bool SpellCastingModeActive;
        private SpellType activeSpell;
        private Vector3 spellTargetPosition;
        private IDictionary<SpellType, float> spellCooldownsLocalCopy = new Dictionary<SpellType, float> { { SpellType.LIGHTNING, 0f }, { SpellType.RAIN, 0f } };
        private RaycastHit hit;
        private Ray ray;

        private PlayerInputListener playerInputListener;
        private PlayerAnimController playerAnimController;
        private SpellcastAudioTriggers spellcastAudioTriggers;

        private void OnEnable()
        {
            playerInputListener = GetComponent<PlayerInputListener>();
            playerAnimController = GetComponent<PlayerAnimController>();
            spellcastAudioTriggers = GetComponent<SpellcastAudioTriggers>();
            CreateSpellAOEIndicatorInstance();
        }

        private void OnDisable()
        {
            Destroy(spellAOEIndicatorInstance);
        }

        private void CreateSpellAOEIndicatorInstance()
        {
            spellAOEIndicatorInstance = Instantiate(ResourceRegistry.SpellAOEIndicatorPrefab);
            spellAOEIndicatorInstance.transform.localScale = new Vector3(SimulationSettings.PlayerSpellAOEDiameter, 1f, SimulationSettings.PlayerSpellAOEDiameter);
            UpdateSpellAOEIndicatorVisibility();
        }

        private void UpdateSpellAOEIndicatorVisibility()
        {
            spellAOEIndicatorInstance.SetActive(SpellCastingModeActive);
        }
        
        public void ActivateSpellCastingMode(SpellType spellType)
        {
            SpellCastingModeActive = true;
            activeSpell = spellType;
            UpdateSpellTargetPosition();
            UpdateSpellAOEIndicatorVisibility();
        }

        public void DeactivateSpellCastingMode()
        {
            SpellCastingModeActive = false;
            UpdateSpellAOEIndicatorVisibility();
        }

        public void AttemptToCastSpell()
        {
            if (!SpellCastingModeActive || spellCooldownsLocalCopy[activeSpell] > 0f)
            {
                return;
            }
            StartCoroutine(CastSpellAfterAnimationBuffer(SimulationSettings.PlayerCastAnimationBuffer));
        }

        private IEnumerator CastSpellAfterAnimationBuffer(float castAnimationBuffer)
        {
            playerAnimController.AnimateSpellCast();
            spellcastAudioTriggers.TriggerSpellChannelAudio();
            DisableMovementForSpellcast();
            yield return new WaitForSeconds(castAnimationBuffer);
            CastSpell();
        }

        private void CastSpell()
        {
            var spellCastRequest = new SpellCastRequest(activeSpell, spellTargetPosition.ToCoordinates());
            SpatialOS.Commands.SendCommand(clientAuthorityCheck, Spells.Commands.SpellCastRequest.Descriptor, spellCastRequest, gameObject.EntityId(), response => { });
            SetLocalSpellCooldown(activeSpell, SimulationSettings.SpellCooldown);
            DeactivateSpellCastingMode();
        }

        private void DisableMovementForSpellcast()
        {
            playerInputListener.DisableInputForSpellcast();
        }

        private void Update()
        {
            UpdateSpellTargetPosition();
            ReduceCooldowns(Time.deltaTime);
            UpdateSpellsPanelCooldowns();
        }

        private void UpdateSpellTargetPosition()
        {
            if (!SpellCastingModeActive)
            {
                return;
            }
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, SimulationSettings.MaxRaycastDistance, (1 << LayerMask.NameToLayer(SimulationSettings.TerrainLayerName))))
            {
                spellTargetPosition = hit.point;
                var zFightOffset = new Vector3(0.0f, 0.1f, 0.0f);
                spellAOEIndicatorInstance.transform.position = hit.point + zFightOffset;
            }
        }

        private void SetLocalSpellCooldown(SpellType spellType, float value)
        {
            spellCooldownsLocalCopy[spellType] = value;
        }

        private void ReduceCooldowns(float deltaTime)
        {
            var enumerator = new List<SpellType>(spellCooldownsLocalCopy.Keys).GetEnumerator();
            while (enumerator.MoveNext())
            {
                spellCooldownsLocalCopy[enumerator.Current] = Mathf.Max(spellCooldownsLocalCopy[enumerator.Current] - deltaTime, 0f);
            }
        }

        private void UpdateSpellsPanelCooldowns()
        {
            SpellsPanelController.SetLightningIconFill(1f - spellCooldownsLocalCopy[SpellType.LIGHTNING] / SimulationSettings.SpellCooldown);
            SpellsPanelController.SetRainIconFill(1f - spellCooldownsLocalCopy[SpellType.RAIN] / SimulationSettings.SpellCooldown);
        }

        public float GetLocalSpellCooldown(SpellType spellType)
        {
            return spellCooldownsLocalCopy[spellType];
        }
    }
}
