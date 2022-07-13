using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchemy.Inventory
{
    [RequireComponent(typeof(AudioSource))]
    public class Chest : InteractiveObject
    {
        private SpriteRenderer Renderer;
        public ItemInstance[] LootTable;
        public Sprite OpenedSprite;

        public bool isDynamic;
        private DungeonManager dungeonManager;
        public bool isUsed = false;

        private void Awake()
        {
            Renderer = GetComponent<SpriteRenderer>();
            dungeonManager = FindObjectOfType<DungeonManager>();
        }

        private IEnumerator AddLoot(List<ItemInstance> LootedItems)
        {
            yield return new WaitForSecondsRealtime(0.15f); // This stops the players previous BACK input

            foreach (ItemInstance Item in LootedItems)
            {
                Inventory.AddItem(Item.Base, Item.Count);
            }
        }

        public override void Interact()
        {
            base.Interact();

            isUsed = true;

            GetComponent<AudioSource>().Play();
            string GotString = "You obtained:\n";

            List<ItemInstance> LootedItems = new List<ItemInstance>();
            List<ItemInstance> Loot = LootTable.ToList();

            if (isDynamic)
            {
                Loot.Clear();

                Stats.ActorStats Player = PlayerMovement.Instance.GetComponent<Stats.ActorStats>();

                foreach (ItemInstance I in LootTable)
                {
                    if (I != null && I.Base != null && Player != null && Player.Skills != null)
                    {
                        ItemInstance Itm = I.CreateInstance();

                        bool Add = true;

                        if ((I.Base.SkillToLearn != null && Player.Skills.Contains(I.Base.SkillToLearn))
                            || (Inventory.GetItem(I.Base) != null && Inventory.GetItem(I.Base).Count > I.Base.AutoBalanceThreshold))
                        {
                            Itm.DropProbability /= 2; // Lower the probability of recieving this item, to make way for others
                            
                            if (I.Base.SkillToLearn != null && Player.Skills.Contains(I.Base.SkillToLearn))
                            {
                                Add = false;
                            }
                            if (I.Base.isDungeonSkill)
                            {
                                switch (I.Base.dungeonSkillType)
                                {
                                    case DungeonManager.DungeonSkillType.Cartography:
                                        if (DungeonManager.hasCartographer)
                                            Add = false;
                                        break;
                                    case DungeonManager.DungeonSkillType.Boss_Sense:
                                        if (DungeonManager.hasBossSense)
                                            Add = false;
                                        break;
                                    case DungeonManager.DungeonSkillType.Scroll_Sense:
                                        if (DungeonManager.hasScrollSense)
                                            Add = false;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (Inventory.GetItem(I.Base) == null || Inventory.GetItem(I.Base).Count == 0)
                            {
                                Itm.DropProbability *= 2; // Raise the probability of the player gaining an item they don't have
                                Itm.MinCount = Mathf.RoundToInt(Itm.MinCount * 1.4f);
                                Itm.MaxCount = Mathf.RoundToInt(Itm.MaxCount * 2);
                            }
                        }

                        if (Add)
                            Loot.Add(Itm);
                    }
                }
            }


            if (Loot.Count > 0)
            {
                bool ChestHasSkill = false;

                foreach (ItemInstance Item in Loot)
                {
                    if (Item.Base != null)
                    {
                        float Chance = Random.Range(0f, 1f);

                        if (Chance <= Item.DropProbability)
                        {
                            if (Item.UseRandomAmount)
                                Item.Count = Random.Range(Item.MinCount, Item.MaxCount);

                            if ((!Item.Base.SkillToLearn && !Item.Base.isDungeonSkill) ||
                                ((Item.Base.SkillToLearn || Item.Base.isDungeonSkill) && !ChestHasSkill))
                            {
                                LootedItems.Add(Item);
                                GotString += $"{Item.Count}x {Item.Base.ItemName}\n";
                            }
                            if (Item.Base.SkillToLearn != null || Item.Base.isDungeonSkill)
                                ChestHasSkill = true;

                        }
                    }
                }
            }
            else
            {
                GotString = "You find nothing inside...";
            }

            Renderer.sprite = OpenedSprite;
            DialogueManager.OnDialogueClose += () => 
            {
                StartCoroutine(AddLoot(LootedItems));
            };

            DialogueManager.ShowMessage(GotString);
            LootTable = new ItemInstance[] { };

            ReadyToInteract = false;
        }
    }
}