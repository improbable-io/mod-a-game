using System.Collections;
using Assets.Gamelogic.Core;
using Improbable;
using Improbable.Building;
using Improbable.Collections;
using Improbable.Core;
using Improbable.Global;
using Improbable.Npc;
using Improbable.Tree;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    public class NPCLumberjackBehaviour : MonoBehaviour
    {
        [Require] private NPCLumberjack.Writer lumberjack;
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private Inventory.Reader inventory;
        [Require] private TeamKnowledge.Reader teamKnowledge;
        [Require] private TeamAssignment.Reader teamAssignment;

        private TargetNavigationBehaviour navigation;
        private InventoryBehaviour inventoryInterface;

        private enum TargetTypes { Tree, Stockpile }

        private void Awake()
        {
            navigation = GetComponent<TargetNavigationBehaviour>();
            inventoryInterface = GetComponent<InventoryBehaviour>();
        }

        private void OnEnable()
        {
            targetNavigation.ComponentUpdated += OnTargetNavigationUpdated;
            lumberjack.ComponentUpdated += OnNpcLumberjackComponentUpdated;

            OnChangeState(lumberjack.Data.currentState, lumberjack.Data.targetEntity);
        }

        private void OnDisable()
        {
            targetNavigation.ComponentUpdated -= OnTargetNavigationUpdated;
            lumberjack.ComponentUpdated -= OnNpcLumberjackComponentUpdated;

            StopAllCoroutines();
        }

        private void OnNpcLumberjackComponentUpdated(NPCLumberjack.Update update)
        {
            if (update.currentState.HasValue)
            {
                OnChangeState(update.currentState.Value, lumberjack.Data.targetEntity);
            }
        }

        private void OnTargetNavigationUpdated(TargetNavigation.Update navigationUpdate)
        {
            for (var i = 0; i < navigationUpdate.reachedTarget.Count; i++)
            {
                if (lumberjack.Data.currentState == LumberjackFSMState.StateEnum.ON_FIRE)
                {
                    StartMovingTowardsPosition(transform.position + (Random.insideUnitSphere * SimulationSettings.OnFireWaypointDistance).FlattenVector());
                }
                else
                {
                    StartCoroutine(AttemptInteractionAfter(SimulationSettings.NPCInteractionDelayTime));
                }
            }
        }

        private void OnChangeState(LumberjackFSMState.StateEnum newState, EntityId targetEntityId)
        {
            StopAllCoroutines();

            switch (newState)
            {
                case LumberjackFSMState.StateEnum.IDLE:
                    navigation.StopNavigation();
                    DecideWhereToMove();
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_TREE:
                    break;
                case LumberjackFSMState.StateEnum.HARVESTING:
                    StartCoroutine(BufferHarvestingAnimation(SimulationSettings.NPCChoppingAnimationBuffer, targetEntityId));
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_STOCKPILE:
                    break;
                case LumberjackFSMState.StateEnum.STOCKPILING:
                    StartCoroutine(BufferDroppingAnimation(SimulationSettings.NPCStockpilingAnimationBuffer, targetEntityId));
                    break;
                case LumberjackFSMState.StateEnum.ON_FIRE:
                    navigation.StopNavigation();
                    StartMovingTowardsPosition(transform.position + (Random.insideUnitSphere * SimulationSettings.OnFireWaypointDistance).FlattenVector());
                    break;
            }
        }

        private IEnumerator AttemptInteractionAfter(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            AttemptInteractionWithTarget();
        }

        private void AttemptInteractionWithTarget()
        {
            EntityId targetEntityId = lumberjack.Data.targetEntity;
            switch (lumberjack.Data.currentState)
            {
                case LumberjackFSMState.StateEnum.MOVING_TO_TREE:
                    if (WithinInteractionRange(targetEntityId, SimulationSettings.NPCChoppingRange))
                    {
                        ChangeStateTo(LumberjackFSMState.StateEnum.HARVESTING, lumberjack.Data.targetEntity);
                        return;
                    }
                    break;
                case LumberjackFSMState.StateEnum.MOVING_TO_STOCKPILE:
                    if (WithinInteractionRange(targetEntityId, SimulationSettings.NPCStockpilingRange))
                    {
                        ChangeStateTo(LumberjackFSMState.StateEnum.STOCKPILING, lumberjack.Data.targetEntity);
                        return;
                    }
                    break;
            }
            ChangeStateTo(LumberjackFSMState.StateEnum.IDLE);
        }

        private IEnumerator BufferHarvestingAnimation(float bufferTime, EntityId targetEntity)
        {
            yield return new WaitForSeconds(bufferTime);
            AttemptToHarvestTree(targetEntity);
        }

        private IEnumerator BufferDroppingAnimation(float bufferTime, EntityId targetEntity)
        {
            yield return new WaitForSeconds(bufferTime);
            AddToStockpile(targetEntity);
        }

        private void DecideWhereToMove()
        {
            TargetTypes targetType; 
            if (HasResourcesToStockpile())
            {
                targetType = TargetTypes.Stockpile;
            }
            else
            {
                targetType = TargetTypes.Tree;
            }

            GameObject targetEntity = SearchForNearby(targetType);
            if (targetEntity != null)
            {
                StartMovingTowardsEntity(targetType, targetEntity);
            }
            else
            {
                StartCoroutine(RetryMoveDecision(10.0f));

                var hqPosition = teamKnowledge.Data.teamHqLocations[(int) teamAssignment.Data.teamId].ToVector3();
                var offset = Random.insideUnitSphere;
                offset = (offset*10.0f) + (offset.normalized*5.0f);
                StartMovingTowardsPosition(hqPosition + offset);
            }
        }

        private IEnumerator RetryMoveDecision(float delay)
        {
            yield return new WaitForSeconds(delay);
            DecideWhereToMove();
        }

        private void StartMovingTowardsPosition(Vector3 position)
        {
            navigation.SetNavigation(position.ToCoordinates());
        }

        private void StartMovingTowardsEntity(TargetTypes targetType, GameObject target)
        {
            switch (targetType)
            {
                case TargetTypes.Tree:
                    lumberjack.Send(new NPCLumberjack.Update().SetCurrentState(LumberjackFSMState.StateEnum.MOVING_TO_TREE).SetTargetEntity(target.EntityId()));
                    break;
                case TargetTypes.Stockpile:
                    lumberjack.Send(new NPCLumberjack.Update().SetCurrentState(LumberjackFSMState.StateEnum.MOVING_TO_STOCKPILE).SetTargetEntity(target.EntityId()));
                    break;
                default:
                    Debug.LogError("I'm moving towards something which isn't a tree or stockpile, help...");
                    break;
            }
            navigation.SetNavigation(target.transform.position.ToCoordinates());
        }

        private GameObject SearchForNearby(TargetTypes targetType)
        {
            string targetTag = GetTargetTag(targetType);
            GameObject[] candidateTargets = GameObject.FindGameObjectsWithTag(targetTag);

            if (targetType == TargetTypes.Stockpile)
            {
                candidateTargets = FilterForSameTeam(candidateTargets);
            }

            if (candidateTargets.Length > 0)
            {
                return ChooseNearest(candidateTargets);
            }
            return null;
        }

        private GameObject[] FilterForSameTeam(GameObject[] candidateTargets)
        {
            ArrayList targetsOnSameTeam = new ArrayList();
            for (int candidateNum = 0; candidateNum < candidateTargets.Length; candidateNum++)
            {
                var currentCandidate = candidateTargets[candidateNum];
                var targetTeamAssignment = currentCandidate.GetComponent<TeamAssignmentVisualizer>();
                if (targetTeamAssignment != null && targetTeamAssignment.TeamId == teamAssignment.Data.teamId)
                {
                    targetsOnSameTeam.Add(currentCandidate);
                }
            }
            return targetsOnSameTeam.ToArray(typeof(GameObject)) as GameObject[];
        }

        private string GetTargetTag(TargetTypes targetType)
        {
            switch (targetType)
            {
                 case TargetTypes.Tree:
                    return SimulationSettings.HealthyTreeTag;
                case TargetTypes.Stockpile:
                    return SimulationSettings.StockpileTag;
                default:
                    return "";
            }
        }

        private GameObject ChooseNearest(GameObject[] nearbyObjects)
        {
            GameObject closestObject = null;
            float closestObjectDistance = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            List<GameObject> candidateObjects = new List<GameObject>(SimulationSettings.NPCCandidateTrees);

            for (int nearbyObjectNum = 0; nearbyObjectNum < nearbyObjects.Length; nearbyObjectNum++)
            {
                GameObject nearbyObject = nearbyObjects[nearbyObjectNum];
                float distanceToObject = Vector3.Distance(nearbyObject.transform.position, currentPosition);
                if (distanceToObject < SimulationSettings.NPCResourceGatheringPreferredDistance)
                {
                    candidateObjects.Add(nearbyObject);
                    if (candidateObjects.Count == candidateObjects.Capacity)
                    {
                        return candidateObjects[Mathf.RoundToInt(Random.value * (candidateObjects.Count - 1))];
                    }
                }
                if (distanceToObject < closestObjectDistance)
                {
                    closestObject = nearbyObject;
                    closestObjectDistance = distanceToObject;
                }
            }

            if (candidateObjects.Count > 0)
            {
                return candidateObjects[Mathf.RoundToInt(Random.value * (candidateObjects.Count - 1))];
            }

            return closestObject;
        }

        private bool HasResourcesToStockpile()
        {
            return inventory.Data.resources > 0;
        }    

        private void AttemptToHarvestTree(EntityId treeEntityId)
        {
            if (TargetTreeIsHealthy(treeEntityId))
            {
                SpatialOS.Commands.SendCommand(lumberjack, 
                    Harvestable.Commands.YieldHarvest.Descriptor,
                    new YieldHarvestRequest(harvester : gameObject.EntityId()), treeEntityId, OnHarvestResponse);
            }
            else
            {
                ChangeStateTo(LumberjackFSMState.StateEnum.IDLE);
            }
        }

        private bool TargetTreeIsHealthy(EntityId treeEntityId)
        {
            string currentTreeTag = SpatialOS.Universe.Get(treeEntityId).UnderlyingGameObject.tag;
            return currentTreeTag == SimulationSettings.HealthyTreeTag;
        }

        private void OnHarvestResponse(ICommandCallbackResponse<HarvestResponse> response)
        {
            if (response.StatusCode == StatusCode.Failure)
            {
                Debug.LogError("NPC failed to receive Harvest response");
                return;
            }
            inventoryInterface.AddToInventory(response.Response.Value.resourcesTaken);
            StartCoroutine(ChangeStateAfter(SimulationSettings.NPCChoppingAnimationFinishTime, LumberjackFSMState.StateEnum.IDLE));
        }

        private IEnumerator ChangeStateAfter(float delayTime, LumberjackFSMState.StateEnum newState)
        {
            yield return new WaitForSeconds(delayTime);
            ChangeStateTo(newState);
        }

        private void AddToStockpile(EntityId stockpileEntityId)
        {
            int resourcesToAdd = inventoryInterface.Size();
            SpatialOS.Commands.SendCommand(lumberjack, StockpileDepository.Commands.AddResource.Descriptor,
                new AddResource(resourcesToAdd), stockpileEntityId, response => OnStockpileResponse(response, resourcesToAdd));
        }

        private void OnStockpileResponse(ICommandCallbackResponse<Nothing> response, int resourcesToAdd)
        {
            if (response.StatusCode == StatusCode.Failure)
            {
                Debug.LogError("NPC failed to receive Stockpile response");
                return;
            }
            inventoryInterface.RemoveFromInventory(resourcesToAdd);
            StartCoroutine(ChangeStateAfter(SimulationSettings.NPCStockpilingAnimationFinishTime, LumberjackFSMState.StateEnum.IDLE));
        }

        private bool WithinInteractionRange(EntityId targetEntity, float interactionRange)
        {
            GameObject target = SpatialOS.Universe.Get(targetEntity).UnderlyingGameObject;
            return Vector3.Distance(transform.position, target.transform.position) <= interactionRange;
        }

        public void ChangeStateTo(LumberjackFSMState.StateEnum newState)
        {
            ChangeStateTo(newState, EntityId.InvalidEntityId);
        }

        private void ChangeStateTo(LumberjackFSMState.StateEnum newState, EntityId targetEntityId)
        {
            if (lumberjack != null)
            {
                lumberjack.Send(new NPCLumberjack.Update().SetCurrentState(newState).SetTargetEntity(targetEntityId));
            }
        }
    }
}
