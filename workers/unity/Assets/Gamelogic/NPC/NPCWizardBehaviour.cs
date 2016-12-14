using System;
using System.Collections;
using System.Linq;
using Assets.Gamelogic.Abilities;
using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Assets.Gamelogic.Life;
using Improbable;
using Improbable.Abilities;
using Improbable.Collections;
using Improbable.Global;
using Improbable.Math;
using Improbable.Npc;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using UnityEngine;

namespace Assets.Gamelogic.NPC
{
    [EngineType(EnginePlatform.FSim)]
    public class NPCWizardBehaviour : MonoBehaviour
    {
        [Require] private NPCWizard.Writer wizard;
        [Require] private TargetNavigation.Writer targetNavigation;
        [Require] private TeamKnowledge.Reader teamKnowledge;

        private Coroutine CheckForNearbyEnemiesCoroutine;
        private System.Random random = new System.Random();

        [SerializeField] private SpellsBehaviour spellsBehaviour;
        [SerializeField] private TeamAssignmentVisualizer teamAssignment;
        [SerializeField] private TargetNavigationBehaviour navigation;
        [SerializeField] private Collider[] nearbyColliders = new Collider[32];
        [SerializeField] private WizardFSMState.StateEnum cachedWizardFSMStateEnum;
        [SerializeField] private List<Coordinates> cachedTeamHqCoordinates;

        private void Awake()
        {
            if (navigation == null)
            {
                navigation = GetComponent<TargetNavigationBehaviour>();
            }

            if (teamAssignment == null)
            {
                teamAssignment = GetComponent<TeamAssignmentVisualizer>();
            }

            if (spellsBehaviour == null)
            {
                spellsBehaviour = GetComponent<SpellsBehaviour>();
            }
        }

        private void OnEnable()
        {
            cachedTeamHqCoordinates = teamKnowledge.Data.teamHqLocations;

            targetNavigation.ComponentUpdated += OnTargetNavigationCopmonentUpdate;
            wizard.ComponentUpdated += OnWizardComponentUpdate;
            teamKnowledge.ComponentUpdated += OnTeamKnowledgeComponentUpdate;

            ChangeTo(WizardFSMState.StateEnum.IDLE);
        }

        private void OnDisable()
        {
            CheckForNearbyEnemiesCoroutine = null;

            targetNavigation.ComponentUpdated -= OnTargetNavigationCopmonentUpdate;
            wizard.ComponentUpdated -= OnWizardComponentUpdate;
            teamKnowledge.ComponentUpdated -= OnTeamKnowledgeComponentUpdate;
        }

        private void StartCoroutines()
        {
            if (CheckForNearbyEnemiesCoroutine == null)
            {
                CheckForNearbyEnemiesCoroutine = StartCoroutine(CheckForNearbyEnemiesLoop());
            }
        }

        private void StopCoroutines()
        {
            StopAllCoroutines();
            CheckForNearbyEnemiesCoroutine = null;
        }

        private IEnumerator CheckForNearbyEnemiesLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(SimulationSettings.NPCWizardDetectEnemiesInterval);

                if (wizard == null)
                {
                    yield break;
                }

                if (wizard.Data.currentState == WizardFSMState.StateEnum.MOVING_TO_POSITION ||
                    wizard.Data.currentState == WizardFSMState.StateEnum.MOVING_TO_TARGET ||
                    wizard.Data.currentState == WizardFSMState.StateEnum.IDLE)
                {
                    var targetToDefend = FindNearestTarget(SimulationSettings.NPCWizardSearchToAttackRadius, IsTargetDefendable);
                    if (targetToDefend != null && random.NextDouble() < SimulationSettings.NPCWizardDefensivePriority)
                    {
                        NavigateToTarget(targetToDefend);
                    }
                    else
                    {
                        var targetToAttack = FindNearestTarget(SimulationSettings.NPCWizardSearchToAttackRadius, IsTargetAttackable);

                        if (targetToAttack != null)
                        {
                            NavigateToTarget(targetToAttack);
                        }
                        else 
                        {
                            if (targetToDefend != null)
                            {
                                NavigateToTarget(targetToDefend);
                            }
                        }
                    }
                }
            }
        }

        private void OnTargetNavigationCopmonentUpdate(TargetNavigation.Update update)
        {
            for (var i = 0; i < update.reachedTarget.Count; i++)
            {
                if (wizard.Data.currentState == WizardFSMState.StateEnum.ON_FIRE)
                {
                    NavigateToPosition(transform.position + (UnityEngine.Random.insideUnitSphere*SimulationSettings.OnFireWaypointDistance).FlattenVector());
                }
                else
                {
                    AttemptInteractionWithTarget();
                }
            }
        }

        private void NavigateToPosition(Vector3 position)
        {
            navigation.SetNavigation(position.ToCoordinates());
        }

        private void AttemptInteractionWithTarget()
        {
            EntityId targetEntityId = wizard.Data.targetEntity;
            if (!WithinInteractionRange(targetEntityId))
            {
                ChangeTo(WizardFSMState.StateEnum.IDLE);
            }
            else
            {
                switch (wizard.Data.currentState)
                {
                    case WizardFSMState.StateEnum.MOVING_TO_TARGET:
                        if (SpatialOS.Universe.ContainsEntity(wizard.Data.targetEntity))
                        {
                            var targetEntity = SpatialOS.Universe.Get(targetEntityId);
                            if (IsTargetAttackable(targetEntity.UnderlyingGameObject))
                            {
                                ChangeTo(WizardFSMState.StateEnum.ATTACKING_TARGET, wizard.Data.targetEntity);
                            }
                            else if (IsTargetDefendable(targetEntity.UnderlyingGameObject))
                            {
                                ChangeTo(WizardFSMState.StateEnum.DEFENDING_TARGET, wizard.Data.targetEntity);
                            }
                            else
                            {
                                ChangeTo(WizardFSMState.StateEnum.IDLE);
                            }
                        }
                        break;
                    case WizardFSMState.StateEnum.MOVING_TO_POSITION:
                        ChangeTo(WizardFSMState.StateEnum.IDLE);
                        break;
                }
            }  
        }

        private void OnTeamKnowledgeComponentUpdate(TeamKnowledge.Update update)
        {
            if (update.teamHqLocations.HasValue)
            {
                cachedTeamHqCoordinates = update.teamHqLocations.Value;
            }
        }

        private void OnWizardComponentUpdate(NPCWizard.Update update)
        {
            if (update.currentState.HasValue)
            {
                cachedWizardFSMStateEnum = update.currentState.Value;
                OnStateChanged(cachedWizardFSMStateEnum, wizard.Data.targetEntity);    
            }
        }

        private void OnStateChanged(WizardFSMState.StateEnum state, EntityId target)
        {
            switch (state)
            {
                case WizardFSMState.StateEnum.IDLE:
                    navigation.StopNavigation();
                    Idle();
                    StartCoroutines();
                    break;
                case WizardFSMState.StateEnum.MOVING_TO_TARGET:
                    break;
                case WizardFSMState.StateEnum.MOVING_TO_POSITION:
                    break;
                case WizardFSMState.StateEnum.ATTACKING_TARGET:
                    TrySpellCast(target, SpellType.LIGHTNING);
                    break;
                case WizardFSMState.StateEnum.DEFENDING_TARGET:
                    TrySpellCast(target, SpellType.RAIN);
                    break;
                case WizardFSMState.StateEnum.ON_FIRE:
                    StopCoroutines();
                    navigation.StopNavigation();
                    NavigateToPosition(transform.position + (UnityEngine.Random.insideUnitSphere * SimulationSettings.OnFireWaypointDistance).FlattenVector());
                    break;
            }
        }

        private void Idle()
        {
            var enemyTeamId = GetRandomEnemyTeamId();
            if (enemyTeamId >= 0)
            {
                Coordinates approximateHQPosition = cachedTeamHqCoordinates[enemyTeamId];
                approximateHQPosition.X += -SimulationSettings.HQSpawnRadius / 2 + SimulationSettings.SpawnOffsetFactor * random.NextDouble();
                approximateHQPosition.Z += -SimulationSettings.HQSpawnRadius / 2 + SimulationSettings.SpawnOffsetFactor * random.NextDouble();

                NavigateToPosition(approximateHQPosition);

                wizard.Send(new NPCWizard.Update()
                    .SetCurrentState(WizardFSMState.StateEnum.MOVING_TO_POSITION)
                    .SetTargetEntity(EntityId.InvalidEntityId));
            }
        }

        private int GetRandomEnemyTeamId()
        {
            if (SimulationSettings.TeamHQLocations.Length >= 2)
            {
                int[] teamIds = Enumerable.Range(0, SimulationSettings.TeamHQLocations.Length).ToArray();
                int[] randomizedTeamIds = teamIds.OrderBy(x => random.Next()).ToArray();
                for (int i = 0; i < randomizedTeamIds.Length; i++)
                {
                    if (randomizedTeamIds[i] != teamAssignment.TeamId)
                    {
                        return randomizedTeamIds[i];
                    }
                }
            }

            return -1;
        }

        private void NavigateToPosition(Coordinates coordinates)
        {
            navigation.SetNavigation(coordinates);
        }

        private void NavigateToTarget(GameObject target)
        {
            Vector3 targetPosition = target.transform.position;

            wizard.Send(new NPCWizard.Update()
                .SetCurrentState(WizardFSMState.StateEnum.MOVING_TO_TARGET)
                .SetTargetEntity(target.EntityId()));

            NavigateToPosition(targetPosition.ToCoordinates());
        }

        private GameObject FindNearestTarget(float radius, Func<GameObject, bool> conditionForSuccess)
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, nearbyColliders, ~(1 << LayerMask.NameToLayer("Tree")));

            GameObject closestTarget = null;
            float minimumDistanceFound = Mathf.Infinity;

            for (int i = 0; i < count; i++)
            {
                var entity = nearbyColliders[i].transform.gameObject.GetEntityObject();
                if (entity == null)
                {
                    continue;
                }

                var targetObject = entity.UnderlyingGameObject;
                var distance = (targetObject.transform.position - transform.position).sqrMagnitude;

                if (distance < minimumDistanceFound && conditionForSuccess(targetObject))
                {
                    minimumDistanceFound = distance;
                    closestTarget = targetObject;
                }
            }

            return closestTarget;
        }

        private bool IsTargetAttackable(GameObject target)
        {
            var targetTeamAssignment = target.GetComponent<TeamAssignmentVisualizer>();
            var targetFlammable = target.GetComponent<FlammableBehaviour>();
            var targetHealth = target.GetComponent<HealthVisualizer>();

            return teamAssignment != null && targetTeamAssignment != null && 
                   teamAssignment.TeamId != targetTeamAssignment.TeamId &&
                   targetFlammable != null &&
                   targetHealth != null && targetHealth.CurrentHealth > 0;
        }

        private bool IsTargetDefendable(GameObject target)
        {
            var targetTeamAssignment = target.GetComponent<TeamAssignmentVisualizer>();
            var targetFlammable = target.GetComponent<FlammableBehaviour>();
            var targetHealth = target.GetComponent<HealthVisualizer>();

            return teamAssignment != null && targetTeamAssignment != null && 
                   teamAssignment.TeamId == teamAssignment.TeamId &&
                   targetFlammable != null && targetFlammable.IsOnFire && 
                   targetHealth != null && targetHealth.CurrentHealth > 0;
        }

        private void TrySpellCast(EntityId targetEntityId, SpellType spellType)
        {
            if (SpatialOS.Universe.ContainsEntity(targetEntityId))
            {
                var targetEntity = SpatialOS.Universe.Get(targetEntityId);
                spellsBehaviour.CastSpell(spellType, targetEntity.UnderlyingGameObject.transform.position);
            }

            StartCoroutine(ChangeAfter(WizardFSMState.StateEnum.IDLE, SimulationSettings.PlayerCastAnimationTime));
        }

        private IEnumerator ChangeAfter(WizardFSMState.StateEnum newState, float playerCastAnimationTime)
        {
            yield return new WaitForSeconds(playerCastAnimationTime);
            ChangeTo(newState);
        }

        private bool WithinInteractionRange(EntityId targetEntity)
        {
            var gameobj = SpatialOS.Universe.Get(targetEntity);
            var outOfRange = gameobj == null;
            if (outOfRange)
            {
                return false;
            }

            GameObject target = gameobj.UnderlyingGameObject;
            return Vector3.Distance(transform.position, target.transform.position) < SimulationSettings.NPCWizardInteractionRange;
        }

        public void ChangeTo(WizardFSMState.StateEnum newState)
        {
            ChangeTo(newState, EntityId.InvalidEntityId);
        }

        private void ChangeTo(WizardFSMState.StateEnum newState, EntityId targetEntityId)
        {
            if (wizard != null)
            {
                wizard.Send(new NPCWizard.Update()
                .SetCurrentState(newState)
                .SetTargetEntity(targetEntityId));
            }  
        }
    }
}
