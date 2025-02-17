using SD_GameLoad;
using TMPro;
using UnityEngine;
using SD_Core;
using System;
using UnityEngine.UI;
using System.Collections;

namespace SD_UI
{
    public class SDAbilityInfoUI : SDLogicMonoBehaviour
    {
        [SerializeField] TMP_Text[] abilityLevelText;
        [SerializeField] TMP_Text[] abilityDamageText;
        [SerializeField] TMP_Text[] abilityUpgradeCostText;
        [SerializeField] TMP_Text[] abilityUnlockLevelText;
        [SerializeField] Button[] abilityUpgraderButton;
        [SerializeField] Image[] abilityIcon;
        [SerializeField] GameObject[] lockIcon;
        [SerializeField] GameObject newSkillToast;
        [SerializeField] SDUpgradePingManager pingManager;

        private void OnEnable()
        {
            AddListener(SDEventNames.UpdateAbilityUpgradeUI, UpdateUpgradeAbilityUI);
            AddListener(SDEventNames.UpdateAbilityUnlockedUI, UpdateUnlockedAbilityUI);
            AddListener(SDEventNames.UpdateAllUpgradesButtons, UpdateAllButtonsInteractability);
            AddListener(SDEventNames.NewSkillToast, NewSkillToast);
        }

        private void OnDisable()
        {
            RemoveListener(SDEventNames.UpdateAbilityUpgradeUI, UpdateUpgradeAbilityUI);
            RemoveListener(SDEventNames.UpdateAbilityUnlockedUI, UpdateUnlockedAbilityUI);
            RemoveListener(SDEventNames.UpdateAllUpgradesButtons, UpdateAllButtonsInteractability);
            RemoveListener(SDEventNames.NewSkillToast, NewSkillToast);
        }

        void Start() => UpdateAbilityTabsUI();

        private void UpdateUpgradeAbilityUI(object abilityData)
        {
            AbilityNames abilityName = (AbilityNames)abilityData;
            int index = (int)abilityName;
            var ability = GameLogic.AbilityData.FindAbilityByName(abilityName.ToString());

            if (ability != null)
            {
                abilityLevelText[index].text = $"Lv. {ability.Level:N0}";
                abilityDamageText[index].text = $"Dmg. {ability.Damage.ToReadableNumber()} x{ability.ComboHits}";
                abilityUpgradeCostText[index].text = $"{ability.UpgradeCost:N0}";
                UpdateAllButtonsInteractability();
            }
        }

        private void UpdateUnlockedAbilityUI(object abilityData)
        {
            int currentPoints = GameLogic.PlayerController.GetAbilityPointsAmount();
            AbilityNames abilityName = (AbilityNames)abilityData;
            int index = (int)abilityName;
            var ability = GameLogic.AbilityData.FindAbilityByName(abilityName.ToString());

            if (ability != null)
            {
                abilityUnlockLevelText[index].text = $"UNLOCKS AT\nLV. {ability.UnlockLevel}";
                abilityUnlockLevelText[index].gameObject.SetActive(!ability.IsUnlocked);
                abilityIcon[index].color = ability.IsUnlocked ? Color.white : Color.black;
                lockIcon[index].SetActive(!ability.IsUnlocked);

                UpdateButtonInteractability(ability, index, currentPoints);
                SDDebug.Log($"{ability}'s unlock is {ability.IsUnlocked}");
            }

            else
            {
                SDDebug.LogException($"{ability} is null");
            }
        }

        private void UpdateAllButtonsInteractability(object obj = null)
        {
            int currentPoints = GameLogic.PlayerController.GetAbilityPointsAmount();

            foreach (AbilityNames abilityName in Enum.GetValues(typeof(AbilityNames)))
            {
                int index = (int)abilityName;
                var ability = GameLogic.AbilityData.FindAbilityByName(abilityName.ToString());

                if (ability != null)
                {
                    UpdateButtonInteractability(ability, index, currentPoints);
                }
            }
            pingManager.UpdateAbilityTabPing();
        }

        private void UpdateAbilityTabsUI()
        {
            foreach (AbilityNames abilityName in Enum.GetValues(typeof(AbilityNames)))
            {
                UpdateUnlockedAbilityUI(abilityName);
                UpdateUpgradeAbilityUI(abilityName);
            }
        }

        private void UpdateButtonInteractability(SDAbilityData ability, int index, int currentPoints)
        {
            bool isInteractable = ability.IsUnlocked && currentPoints >= ability.UpgradeCost;
            abilityUpgraderButton[index].interactable = isInteractable;

            pingManager.SetPing(index, isInteractable);
        }

        private void NewSkillToast(object obj = null) => StartCoroutine(ShowAndHideToast());

        private IEnumerator ShowAndHideToast()
        {
            newSkillToast.SetActive(true);
            yield return new WaitForSeconds(2f);
            newSkillToast.SetActive(false);
        }
    }
}
