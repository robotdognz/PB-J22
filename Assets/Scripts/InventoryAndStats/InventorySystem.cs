using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Inventory 
{
    [System.Serializable]
    public class ItemInstance
    {
        public Item Base;
        public int Count;
        [Range(0, 1)]public float DropProbability = 1;

        public ItemInstance(Item Base, int Count)
        {
            this.Base = Base;
            this.Count = Count;
        }
    }

    public static class Inventory
    {
        public static List<ItemInstance> Items
        {
            get; private set;
        } = new List<ItemInstance>();

        public static void AddItem(Item Itm, int Amount = 1)
        {
            bool HadItem = false;
            ItemInstance Holder = null;

            foreach (ItemInstance I in Items)
            {
                if (I.Base == Itm)
                {
                    Holder = I;
                    HadItem = true;
                }
            }

            if (HadItem)
            {
                Holder.Count += Amount;
            }
            else
            {
                Items.Add(new ItemInstance(Itm, Amount));
            }
        }
    }
}