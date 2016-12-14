using System.Collections;
using System.Collections.Generic;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplate;
using Improbable.Building;
using Improbable.Math;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Npc;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Building
{
    public class NPCSpawnerBehaviour : MonoBehaviour
    {
        [Require] private NPCSpawner.Writer npcSpawner;
        [Require] private TeamAssignment.Reader teamAssignment;

        private Coroutine spawnNPCsReduceCooldownCoroutine;

        private static readonly IDictionary<NPCRoleEnum, float> npcRolesToCooldownDictionary = new Dictionary<NPCRoleEnum, float>
        {
            { NPCRoleEnum.LUMBERJACK, SimulationSettings.LumberjackSpawningCooldown },
            { NPCRoleEnum.WIZARD, SimulationSettings.WaizardSpawningCooldown }
        };

        private void OnEnable()
        {
            spawnNPCsReduceCooldownCoroutine = StartCoroutine(SpawnNPCsReduceCooldown(SimulationSettings.SimulationTickInterval));
        }

        private void OnDisable()
        {
            CancelSpawnNPCsReduceCooldownCoroutine();
        }

        private void CancelSpawnNPCsReduceCooldownCoroutine()
        {
            if (spawnNPCsReduceCooldownCoroutine != null)
            {
                StopCoroutine(spawnNPCsReduceCooldownCoroutine);
                spawnNPCsReduceCooldownCoroutine = null;
            }
        }

        private IEnumerator SpawnNPCsReduceCooldown(float interval)
        {
            var npcRoles = new System.Collections.Generic.List<NPCRoleEnum>(npcRolesToCooldownDictionary.Keys);
            var newCooldowns = new Map<NPCRoleEnum, float>(npcSpawner.Data.cooldowns);
            while (true)
            {
                yield return new WaitForSeconds(interval);
                if (!npcSpawner.Data.spawningEnabled)
                {
                    continue;
                }
                for (int i = 0; i < npcRoles.Count; i++)
                {
                    var role = npcRoles[i];
                    if (newCooldowns[role] <= 0f) // todo: this is a workaround for WIT-1374
                    {
                        var spawningOffset = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * SimulationSettings.SpawnOffsetFactor;
                        var spawnPosition = (gameObject.transform.position + spawningOffset).ToCoordinates();
                        SpawnNpc(role, spawnPosition);
                        newCooldowns[role] = npcRolesToCooldownDictionary[role];
                    }
                    else
                    {
                        newCooldowns[role] = Mathf.Max(newCooldowns[role] - interval, 0f);
                    }
                }
                npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(newCooldowns));
            }
        }

        public void SetSpawningEnabled(bool spawningEnabled)
        {
            if (spawningEnabled != npcSpawner.Data.spawningEnabled)
            {
                npcSpawner.Send(new NPCSpawner.Update().SetSpawningEnabled(spawningEnabled));
            }
        }

        private void SpawnNpc(NPCRoleEnum npcRoleEnum, Coordinates position)
        {
            switch (npcRoleEnum)
            {
                case NPCRoleEnum.LUMBERJACK:
                    SpawnLumberjack(position);
                    break;
                case NPCRoleEnum.WIZARD:
                    SpawnWizard(position);
                    break;
            }
        }

        private void SpawnLumberjack(Coordinates position)
        {
            var template = EntityTemplateFactory.CreateNPCLumberjackTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, SimulationSettings.NPCPrefabName, template, result => { });
        }

        private void SpawnWizard(Coordinates position)
        {
            var template = EntityTemplateFactory.CreateNPCWizardSnapshotTemplate(position, teamAssignment.Data.teamId);
            SpatialOS.Commands.CreateEntity(npcSpawner, SimulationSettings.NPCWizardPrefabName, template, result => { });
        }

        public void ResetCooldowns()
        {
            npcSpawner.Send(new NPCSpawner.Update().SetCooldowns(new Map<NPCRoleEnum, float> { { NPCRoleEnum.LUMBERJACK, 0f }, { NPCRoleEnum.WIZARD, 0f } }));
        }
    }
}
