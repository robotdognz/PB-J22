using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alchemy.Stats;

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
        public Text ActorNameLabel;
        public Image ActorPortrait;
        [Space]
        public float ValueLerpSpeed = 0.5f; // It would take two seconds for the value to go from 0 to 1 by default 
        public Text HitpointsLabel;
        public Image HitpointsImage;
        [Space]
        public Text StaminaLabel;
        public Image StaminaImage;

        private IEnumerator UpdateStatLabels()
        {
            while (true) 
            {
                float HealthValue = PlayerStats.HealthPercent;
                float StaminaValue = PlayerStats.StaminaPercent;

                HitpointsImage.fillAmount = Mathf.MoveTowards(HitpointsImage.fillAmount, HealthValue, Time.deltaTime * ValueLerpSpeed);
                StaminaImage.fillAmount = Mathf.MoveTowards(StaminaImage.fillAmount, StaminaValue, Time.deltaTime * ValueLerpSpeed);

                HitpointsLabel.text = $"{Mathf.RoundToInt(HitpointsImage.fillAmount * PlayerStats.MaxHealth)}/{PlayerStats.MaxHealth}";
                StaminaLabel.text = $"{Mathf.RoundToInt(StaminaImage.fillAmount * PlayerStats.MaxStamina)}/{PlayerStats.MaxStamina}";

                if (HitpointsImage.fillAmount == HealthValue && StaminaImage.fillAmount == StaminaValue)
                    break;

                yield return null;
            }
        }

        private void Awake()
        {
            ResetPlayer(); // This is only to be used until the battle system is done

            StartCoroutine(UpdateStatLabels());
        }

        public void ModLevel(int Amount)
        {
            PlayerStats.CurrentLevel += Amount;

            UpdateUI();
            StartCoroutine(UpdateStatLabels());
        }

        public void ResetPlayer()
        {
            PlayerStats.ResetStats();

            StartCoroutine(UpdateStatLabels());
        }

        public void DamagePlayer(int Damage)
        {
            PlayerStats.ModifyHealth(-Damage);

            StartCoroutine(UpdateStatLabels());
        }

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
            ActorNameLabel.text = $"{PlayerStats.PlayerName}\nLevel {PlayerStats.CurrentLevel}";

            for (int I = 0; I < Menus.Length; I++)
                if (I == CurrentMenu)
                    Menus[I].ThisMenu.SetActive(true);
                else
                    Menus[I].ThisMenu.SetActive(false);
        }
    }
}