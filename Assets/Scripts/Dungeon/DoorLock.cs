using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alchemy.Inventory;
using Alchemy.Combat;

namespace Alchemy.Dungeon
{
    public class DoorLock : MonoBehaviour
    {
        public Skill RequiredSkill;
        public Item RequiredItem;
        public bool ConsumesItem = true;
    }
}