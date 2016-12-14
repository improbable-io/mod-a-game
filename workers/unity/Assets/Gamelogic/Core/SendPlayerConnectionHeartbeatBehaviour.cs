using UnityEngine;
using System.Collections;
using Improbable;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Core
{
    /// <summary>
    /// Regularly notifies the simulation manager entity that this client is still alive and connected.
    /// When this times out (in case of an event like a client crash), the simulation manager entity will clean up our player from the world.
    /// </summary>
    [EngineType(EnginePlatform.Client)]
    public class SendPlayerConnectionHeartbeatBehaviour : MonoBehaviour
    {
        [Require] private ClientAuthorityCheck.Writer authCheck;

        private EntityId entityId;
        private Coroutine heartbeatCoroutine;

        private void OnEnable()
        {
            entityId = gameObject.EntityId();
            heartbeatCoroutine = StartCoroutine(HeartbeatLoop());
        }

        private void OnDisable()
        {
            entityId = EntityId.InvalidEntityId;
            StopCoroutine(heartbeatCoroutine);
        }

        private void SendHeartbeat()
        {
            SpatialOS.Commands.SendCommand(authCheck, Heartbeat.Commands.Heartbeat.Descriptor, new Nothing(), entityId, _ => { });
        }

        private IEnumerator HeartbeatLoop()
        {
            while (true)
            {
                SendHeartbeat();
                yield return new WaitForSeconds(SimulationSettings.HeartbeatInterval);
            }
        }
    }
}