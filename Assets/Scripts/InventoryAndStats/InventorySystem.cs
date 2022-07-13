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
        public bool UseRandomAmount;
        public int MinCount = 1;
        public int MaxCount = 3;

        public ItemInstance(Item Base, int Count, bool UseRandomAmount = false, int MinCount = 1, int MaxCount = 3, float DropProbability = 1)
        {
            this.Base = Base;
            this.Count = Count;
            this.UseRandomAmount = UseRandomAmount;
            this.MinCount = MinCount;
            this.MaxCount = MaxCount;
            this.DropProbability = DropProbability;
        }

        public ItemInstance CreateInstance()
        {
            return new ItemInstance(Base, Count, UseRandomAmount, MinCount, MaxCount, DropProbability);
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

        public static ItemInstance GetItem(Item Base)
        {
            foreach (ItemInstance I in Itms) 
            {
                if (Base == I.Base)
                    return I;
            }

            return null;
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

                if (Itm.isDungeonSkill)
                {
                    Itm.action += DungeonManager.Instance.LearnDungeonSkill;
                    DungeonManager.currentDungeonSkill = Itm.dungeonSkillType;
                    Itm.Activate();
                }
            }
        }
    }
}