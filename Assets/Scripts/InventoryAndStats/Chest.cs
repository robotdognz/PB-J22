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

        public override void Interact()
        {
            base.Interact();

            isUsed = true;

            GetComponent<AudioSource>().Play();
            string GotString = "You obtained:\n";

            if (isDynamic)
            {
                // TODO: dynamic chest contents
            }
            else if (LootTable.Length > 0)
            {
                foreach (ItemInstance Item in LootTable)
                {
                    float Chance = Random.Range(0f, 1f);

                    if (Chance <= Item.DropProbability)
                    {
                        Inventory.AddItem(Item.Base, Item.Count);
                        GotString += $"{Item.Count}x {Item.Base.ItemName}\n";
                    }
                }
            }
            else
            {
                GotString = "You find nothing inside...";
            }

            Renderer.sprite = OpenedSprite;

            DialogueManager.ShowMessage(GotString);
            LootTable = new ItemInstance[] { };
        }
    }
}