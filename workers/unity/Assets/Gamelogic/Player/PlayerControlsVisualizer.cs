using Assets.Gamelogic.Core;
using Improbable.Core;
using Improbable.Player;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Player
{
    public class PlayerControlsVisualizer : MonoBehaviour
    {
        [Require] private FSimAuthorityCheck.Writer fsimAuthorityCheck;
        [Require] private PlayerInfo.Reader playerInfo;
        [Require] private PlayerControls.Reader playerControls;

        private TransformSender transformSender;
        public Vector3 TargetPosition { get { return playerControls.Data.targetPosition.ToVector3(); } }
        private Rigidbody myRigidbody;
        
        private void Awake()
        {
            transformSender = GetComponent<TransformSender>();
            myRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }
        
        private void MovePlayer()
        {
            if (ShouldMovePlayerFSim(TargetPosition, myRigidbody.position))
            {
                if (PlayerMovementCheatSafeguardPassedFSim(TargetPosition, myRigidbody.position))
                {
                    transform.LookAt(TargetPosition);
                    myRigidbody.MovePosition(TargetPosition);
                }
                else
                {
                    transformSender.TriggerTeleport(myRigidbody.position);
                }
            }
        }

        private bool ShouldMovePlayerFSim(Vector3 targetPosition, Vector3 currentPosition)
        {
            return playerInfo.Data.isAlive && (targetPosition - currentPosition).FlattenVector().sqrMagnitude > SimulationSettings.PlayerPositionUpdateMinSqrDistance;
        }

        private bool PlayerMovementCheatSafeguardPassedFSim(Vector3 targetPosition, Vector3 currentPosition)
        {
            var result = (targetPosition - currentPosition).sqrMagnitude < SimulationSettings.PlayerPositionUpdateMaxSqrDistance;
            if (!result)
            {
                Debug.LogError("Player movement cheat safeguard failed on FSim.");
            }
            return result;
        }
    }
}
