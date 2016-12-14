using System.Collections;
using Assets.Gamelogic.Core;
using Improbable.Tree;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    [EngineType(EnginePlatform.Client)]
    public class TreeModelVisualizer : MonoBehaviour
    {
        [Require] private TreeState.Reader treeState;

        public GameObject HealthyTree;
        public GameObject Stump;
        public GameObject BurningTree;
        public GameObject BurntTree;

        private void OnEnable()
        {
            treeState.ComponentUpdated += UpdateVisualization;
            ShowTreeModel(treeState.Data.currentState);
        }

        private void OnDisable()
        {
            treeState.ComponentUpdated -= UpdateVisualization;
        }

        private void UpdateVisualization(TreeState.Update newState)
        {
            ShowTreeModel(newState.currentState.Value);
        }

        private void ShowTreeModel(TreeFSMState currentState)
        {
            switch (currentState)
            {
                case TreeFSMState.HEALTHY:
                    StartCoroutine(TransitionAfter(HealthyTree, SimulationSettings.TreeExtinguishTimeBuffer));
                    break;
                case TreeFSMState.STUMP:
                    StartCoroutine(TransitionAfter(Stump, SimulationSettings.TreeCutDownTimeBuffer));
                    break;
                case TreeFSMState.BURNING:
                    StartCoroutine(TransitionAfter(BurningTree, SimulationSettings.TreeIgnitionTimeBuffer));
                    break;
                case TreeFSMState.BURNT:
                    TransitionTo(BurntTree);
                    break;
            }
        }

        private IEnumerator TransitionAfter(GameObject newModel, float transitionTime)
        {
            yield return new WaitForSeconds(transitionTime);
            TransitionTo(newModel);
        }

        private void TransitionTo(GameObject newModel)
        {
            HideAllModels();
            newModel.SetActive(true);
        }

        private void HideAllModels()
        {
            HealthyTree.SetActive(false);
            Stump.SetActive(false);
            BurningTree.SetActive(false);
            BurntTree.SetActive(false);
        }
    }
}
