using System.Collections.Generic;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.EntityTemplate;
using Improbable;
using Improbable.Core;
using Improbable.Entity.Component;
using Improbable.Global;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Gamelogic.Global
{
    public class PlayerManagementBehaviour : MonoBehaviour
    {
        [Require] private PlayerLifeCycle.Writer playerLifeCycle;
        [Require] private TeamCollection.Reader teamCollection;

        private uint nextAvailableTeamId;
        private readonly IDictionary<string, EntityId> playerEntityIds = new Dictionary<string, EntityId>();

        private void OnEnable()
        {
            playerLifeCycle.CommandReceiver.OnSpawnPlayer += OnSpawnPlayer;
            playerLifeCycle.CommandReceiver.OnDeletePlayer += OnDeletePlayer;
        }

        private void OnDisable()
        {
            playerLifeCycle.CommandReceiver.OnSpawnPlayer -= OnSpawnPlayer;
            playerLifeCycle.CommandReceiver.OnDeletePlayer -= OnDeletePlayer;
        }

        private void OnSpawnPlayer(ResponseHandle<PlayerLifeCycle.Commands.SpawnPlayer, SpawnPlayerRequest, SpawnPlayerResponse> responseHandle)
        {
            if (playerEntityIds.ContainsKey(responseHandle.CallerInfo.CallerWorkerId))
            {
                responseHandle.Respond(new SpawnPlayerResponse(playerEntityIds[responseHandle.CallerInfo.CallerWorkerId]));
                return;
            }
            var assignedTeamId = (nextAvailableTeamId++) % (uint)SimulationSettings.TeamCount; 
            var spawningOffset = new Vector3(Random.value - 0.5f, 0f, Random.value - 0.5f) * SimulationSettings.PlayerSpawnOffsetFactor;
            var hqPos = SimulationSettings.TeamHQLocations[assignedTeamId].ToVector3();
            var initialPosition = hqPos + spawningOffset;

            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerEntityTemplate(responseHandle.CallerInfo.CallerWorkerId, initialPosition.ToCoordinates(), assignedTeamId);
            SpatialOS.Commands.CreateEntity(playerLifeCycle, SimulationSettings.PlayerPrefabName, playerEntityTemplate, response => OnPlayerCreation(responseHandle, response));
        }

        private void OnPlayerCreation(ResponseHandle<PlayerLifeCycle.Commands.SpawnPlayer, SpawnPlayerRequest, SpawnPlayerResponse> responseHandle, ICommandCallbackResponse<EntityId> response)
        {
            if (response.StatusCode != StatusCode.Success)
            {
                Debug.LogError("player spawner failed to create entity: " + response.ErrorMessage);
                return;
            }
            playerEntityIds.Add(responseHandle.CallerInfo.CallerWorkerId, response.Response.Value);
            responseHandle.Respond(new SpawnPlayerResponse(response.Response.Value));
        }

        private void OnDeletePlayer(ResponseHandle<PlayerLifeCycle.Commands.DeletePlayer, DeletePlayerRequest, Nothing> responseHandle)
        {
            if (playerEntityIds.ContainsKey(responseHandle.CallerInfo.CallerWorkerId))
            {
                var entityId = playerEntityIds[responseHandle.CallerInfo.CallerWorkerId];
                SpatialOS.Commands.DeleteEntity(playerLifeCycle, entityId, result =>
                {
                    if (result.StatusCode != StatusCode.Success)
                    {
                        Debug.LogErrorFormat("failed to delete inactive entity {0} with error message: {1}", entityId, result.ErrorMessage);
                        return;
                    }
                });
                playerEntityIds.Remove(responseHandle.CallerInfo.CallerWorkerId);
            }
            responseHandle.Respond(new Nothing());
        }
    }
}
