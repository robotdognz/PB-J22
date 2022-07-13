using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Alchemy.Inventory 
{
    [System.Serializable]
    public class ItemInstance
    {
        public Item Base;
        public int Count;
        [Range(0, 1)]public float DropProbability = 1;

        public ItemInstance(Item Base, int Count, float DropProbability = 1)
        {
            this.Base = Base;
            this.Count = Count;
        }

        public ItemInstance CreateInstance()
        {
            return new ItemInstance(Base, Count, DropProbability);
        }
    }

    public static class Inventory
    {
        public static List<ItemInstance> Itms { get; private set; } = new List<ItemInstance>();
        public static List<ItemInstance> Items
        {
            get
            {
                return Itms.OrderBy(o => o.Base.ItemName).ToList();
            }
            private set
            {
                Itms = value;
            }
        }

        public static void AddItem(Item Itm, int Amount = 1)
        {
            if (!Itm.ConsumeOnAcquire)
            {
                bool HadItem = false;
                ItemInstance Holder = null;

                foreach (ItemInstance I in Itms)
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
                    Itms.Add(new ItemInstance(Itm, Amount));
                }
            }
            else
            {
                Itm.UseItem(PlayerMovement.Instance.GetComponent<Stats.ActorStats>());
                DungeonManager.currentDungeonSkill = Itm.dungeonSkillType;
                Itm.Activate();
            }
        }
    }
}