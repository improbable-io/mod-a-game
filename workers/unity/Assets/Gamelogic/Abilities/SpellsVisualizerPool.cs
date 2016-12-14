using System.Collections.Generic;
using UnityEngine;
using Assets.Gamelogic.UI;
using Improbable.Abilities;

namespace Assets.Gamelogic.Abilities
{
    public class SpellsVisualizerPool : MonoBehaviour
    {
        private static IDictionary<SpellType, Stack<GameObject>> spellEffectInstances = new Dictionary<SpellType, Stack<GameObject>>();
        private static IDictionary<SpellType, GameObject> spellEffectPrefabs = new Dictionary<SpellType, GameObject>();
        private static SpellsVisualizerPool Instance;

        private void Awake()
        {
            Instance = this;

            InitializeSpellType(SpellType.LIGHTNING, ResourceRegistry.LightningEffectPrefab);
            InitializeSpellType(SpellType.RAIN, ResourceRegistry.RainEffectPrefab);
        }

        private void InitializeSpellType(SpellType spellType, GameObject prefab)
        {
            spellEffectInstances[spellType] = new Stack<GameObject>();
            spellEffectPrefabs[spellType] = prefab;
        }

        public static GameObject GetSpellObject(SpellType spellType)
        {
            var stack = spellEffectInstances[spellType];

            if (stack.Count == 0)
            {
                return (GameObject)Instantiate(spellEffectPrefabs[spellType], Instance.transform, true);
            }
            return stack.Pop();
        }

        public static void ReturnSpellObject(SpellType spellType, GameObject visual)
        {
            visual.SetActive(false);
            spellEffectInstances[spellType].Push(visual);
        }
    }
}
