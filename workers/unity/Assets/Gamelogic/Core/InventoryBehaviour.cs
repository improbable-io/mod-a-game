using Improbable.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Core
{
    public class InventoryBehaviour : MonoBehaviour
    {
        [Require] private Inventory.Writer inventory;

        public void AddToInventory(int quantity)
        {
            inventory.Send(new Inventory.Update().SetResources(inventory.Data.resources + quantity));
        }

        public void RemoveFromInventory(int quantity)
        {
            inventory.Send(new Inventory.Update().SetResources(Mathf.Max(0, inventory.Data.resources - quantity)));
        }

        public int Size()
        {
            return inventory.Data.resources;
        }
    }
}