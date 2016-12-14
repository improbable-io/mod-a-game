using Assets.Gamelogic.Core;
using Improbable.Tree;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Tree
{
    [EngineType(EnginePlatform.FSim)]
    public class TreeTagVisualizer : MonoBehaviour {

        [Require] private TreeState.Reader treeStateReader;

        [SerializeField] private string currentTreeTag;

        public string CurrentTreeTag
        {
            get { return currentTreeTag; }
            private set
            {
                currentTreeTag = value;
                gameObject.tag = currentTreeTag;
            }
        }

        private void OnEnable()
        {
            UpdateTreeTag(treeStateReader.Data.currentState);
            treeStateReader.ComponentUpdated += OnTreeStateComponentUpdate;
        }

        private void OnDisable()
        {
            treeStateReader.ComponentUpdated -= OnTreeStateComponentUpdate;
        }

        private void OnTreeStateComponentUpdate(TreeState.Update update)
        {
            if (update.currentState.HasValue)
            {
                UpdateTreeTag(update.currentState.Value);
            }
        }

        private void UpdateTreeTag(TreeFSMState treeFsmState)
        {
            switch (treeFsmState)
            {
                case TreeFSMState.HEALTHY:
                    CurrentTreeTag = SimulationSettings.HealthyTreeTag;
                    break;
                case TreeFSMState.STUMP:
                    CurrentTreeTag = SimulationSettings.TreeStumpTag;
                    break;
                case TreeFSMState.BURNING:
                    CurrentTreeTag = SimulationSettings.BurningTreeTag;
                    break;
                case TreeFSMState.BURNT:
                   CurrentTreeTag = SimulationSettings.BurntTreeTag;
                    break;
                default:
                    return;
            }
        }
    }
}
