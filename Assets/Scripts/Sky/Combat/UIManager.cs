using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Alchemy.Combat
{
    [System.Serializable]
    public class Menu
    {
        public GameObject ThisMenu;
        public int ParentMenu;
    }

    public class UIManager : MonoBehaviour
    {
        public int CurrentMenu { get; private set; }
        [Header("Menus")]
        public Menu[] Menus;

        [Header("Stats")]
        public Text HitpointsLabel;
        public Image HitpointsImage;
        [Space]
        public Text StaminaLabel;
        public Image StaminaImage;

        public void SetMenu(int Menu)
        {
            CurrentMenu = Menu;

            UpdateUI();
        }

        public void OnCancel()
        {
            int OldMenu = CurrentMenu;
            CurrentMenu = Menus[OldMenu].ParentMenu;

            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int I = 0; I < Menus.Length; I++)
                if (I == CurrentMenu)
                    Menus[I].ThisMenu.SetActive(true);
                else
                    Menus[I].ThisMenu.SetActive(false);

            // Implement stats
        }
    }
}