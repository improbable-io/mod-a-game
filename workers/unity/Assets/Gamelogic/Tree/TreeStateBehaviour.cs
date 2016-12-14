using Assets.Gamelogic.Core;
using Assets.Gamelogic.Fire;
using Improbable.Core;
using Improbable.Fire;
using Improbable.Tree;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using System.Collections;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    public class TreeStateBehaviour : MonoBehaviour
    {
        [Require] private TreeStateWriter treeState;
        [Require] private Flammable.Writer flammable;
        [Require] private Harvestable.Reader harvestable;

        private HarvestableBehaviour harvestableInterface;
        private FlammableBehaviour flammableInterface;
        private Coroutine timingCoroutine;

        private void Awake()
        {
            harvestableInterface = GetComponent<HarvestableBehaviour>();
            flammableInterface = GetComponent<FlammableBehaviour>();
        }

        private void OnEnable()
        {
            flammable.ComponentUpdated += FlammableUpdate;
            harvestable.ComponentUpdated += HarvestResourcesUpdated;
        }

        private void OnDisable()
        {
            CancelTimingCoroutine();

            flammable.ComponentUpdated -= FlammableUpdate;
            harvestable.ComponentUpdated -= HarvestResourcesUpdated;
        }

        private void CancelTimingCoroutine()
        {
            if (timingCoroutine != null)
            {
                StopCoroutine(timingCoroutine);
            }
        }

        private void HarvestResourcesUpdated(Harvestable.Update harvestResourceUpdate)
        {
            if (harvestResourceUpdate.resources.HasValue && harvestResourceUpdate.resources.Value == 0)
            {
                SetState(TreeFSMState.STUMP);
            }
        }

        private void FlammableUpdate(Flammable.Update flammableUpdate)
        {
            if (WasBurning() && HasBeenExtinguished(flammableUpdate))
            {
                SetState(TreeFSMState.HEALTHY);
            }
            else if (HasBeenIgnited(flammableUpdate))
            {
                SetState(TreeFSMState.BURNING);
            }
        }

        public void SetState(TreeFSMState newState)
        {
            CancelTimingCoroutine();

            if (!treeState.CurrentState.Equals(newState))
            {
                treeState.Update.CurrentState(newState).FinishAndSend();

                switch (newState)
                {
                    case TreeFSMState.HEALTHY:
                        harvestableInterface.ResetResourceCount();
                        flammable.Send(new Flammable.Update().SetCanBeIgnited(true));
                        break;
                    case TreeFSMState.STUMP:
                        flammable.Send(new Flammable.Update().SetCanBeIgnited(false));
                        timingCoroutine = StartCoroutine(RegrowAfter(SimulationSettings.TreeStumpRegrowthTimeSecs));
                        break;
                    case TreeFSMState.BURNING:
                        timingCoroutine = StartCoroutine(BurnAfter(SimulationSettings.TreeBurningTimeSecs));
                        break;
                    case TreeFSMState.BURNT:
                        flammableInterface.SelfExtinguish(flammable, false);
                        timingCoroutine = StartCoroutine(RegrowAfter(SimulationSettings.BurntTreeRegrowthTimeSecs));
                        break;
                }
            }
        }

        private IEnumerator BurnAfter(int burningTimeSecs)
        {
            yield return new WaitForSeconds(burningTimeSecs);
            SetState(TreeFSMState.BURNT);
        }

        private IEnumerator RegrowAfter(int treeRegrowthTimeSecs)
        {
            yield return new WaitForSeconds(treeRegrowthTimeSecs);
            SetState(TreeFSMState.HEALTHY);
        }

        private bool WasBurning()
        {
            if (treeState == null)
            {
                Debug.LogError("Dodged hitting null in WasBurning");
                return false;
            }

            return treeState.CurrentState.Equals(TreeFSMState.BURNING);
        }

        private bool HasBeenExtinguished(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && !flammableUpdate.isOnFire.Value;
        }

        private bool HasBeenIgnited(Flammable.Update flammableUpdate)
        {
            return flammableUpdate.isOnFire.HasValue && flammableUpdate.isOnFire.Value;
        }
    }
}
