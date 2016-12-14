using System.Collections.Generic;
using UnityEngine;
using Assets.Gamelogic.UI;

namespace Assets.Gamelogic.Core
{
    public class DeathAnimVisualizerPool : MonoBehaviour
    {
        private static Stack<GameObject> deathEffectInstances;
        private static GameObject deathEffectPrefab;
        private static DeathAnimVisualizerPool Instance;

        private void Awake()
        {
            Instance = this;
            InitializePool(ResourceRegistry.DeathEffectPrefab);
        }

        private void InitializePool(GameObject prefab)
        {
            deathEffectInstances = new Stack<GameObject>();
            deathEffectPrefab = prefab;
        }

        public static GameObject GetEffectObject()
        {
            var stack = deathEffectInstances;

            if (stack.Count == 0)
            {
                return (GameObject)Instantiate(deathEffectPrefab, Instance.transform, true);
            }
            return stack.Pop();
        }

        public static void ReturnEffectObject(GameObject visual)
        {
            visual.SetActive(false);
            deathEffectInstances.Push(visual);
        }
    }
}
