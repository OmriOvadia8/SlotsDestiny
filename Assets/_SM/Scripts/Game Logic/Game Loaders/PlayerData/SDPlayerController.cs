using SD_Core;

namespace SD_GameLoad
{
    public class SDPlayerController
    {
        private const double TOTAL_XP_MULTIPLIER_INCREASE = 1.2;
        private const int EXTRA_BOSS_POINTS = 10;
        private bool hasLeveledUp;
        private SDPlayerData PlayerInfo => SDGameLogic.Instance.Player.PlayerData.PlayerInfo;

        #region Player Leveling
        public void AddPlayerXP(double xp)
        {
            PlayerInfo.CurrentXp += xp;
            SDDebug.Log($"Before Level Up: CurrentXp = {PlayerInfo.CurrentXp}, TotalXpRequired = {PlayerInfo.TotalXpRequired}");

            if (PlayerInfo.CurrentXp >= PlayerInfo.TotalXpRequired)
            {
                double tempXPRequired = PlayerInfo.TotalXpRequired;
                PlayerLevelUp();
                PlayerInfo.CurrentXp -= tempXPRequired;
                SDDebug.Log($"After Level Up: CurrentXp = {PlayerInfo.CurrentXp}, TotalXpRequired = {PlayerInfo.TotalXpRequired}, tempXPRequired = {tempXPRequired}");
            }

            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.XPToast, xp);
            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.UpdateXpUI, null);
            SDGameLogic.Instance.Player.SavePlayerData();
        }

        private void PlayerLevelUp()
        {
            SetLevelUpFlag(true);
            PlayerInfo.Level++;
            UpdateTotalXPRequired();
            EarnAbilityPoints(PointsEarnTypes.LevelUp);
            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.LvlUpToast, null);
            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.CheckUnlockAbility, PlayerInfo.Level);
            SDGameLogic.Instance.Player.SavePlayerData();
        }

        private void UpdateTotalXPRequired() => PlayerInfo.TotalXpRequired *= TOTAL_XP_MULTIPLIER_INCREASE;

        public void SetLevelUpFlag(bool value) => hasLeveledUp = value;

        public bool GetLevelUpFlag()
        {
            return hasLeveledUp;
        }
        #endregion

        #region Player Rolls
        public void DecreaseRoll()
        {
            if (GetCurrentRollsAmount() > 0)
            {
                PlayerInfo.CurrentRolls--;
            }

            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.UpdateRollsUI, null);
            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.CheckRollsForSpin, null);
            if(!PlayerInfo.IsRollRegenOn)
            {
                SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.StartRollsRegeneration, null);
            }
            SDGameLogic.Instance.Player.SavePlayerData();
        }

        public void IncreaseRoll(int rolls)
        {
            if (GetCurrentRollsAmount() < PlayerInfo.MaxRolls)
            {
                PlayerInfo.CurrentRolls+= rolls;
            }

            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.UpdateRollsUI, null);
            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.CheckRollsForSpin, null);

            SDGameLogic.Instance.Player.SavePlayerData();
        }

        public int GetCurrentRollsAmount()
        {
            return PlayerInfo.CurrentRolls;
        }

        public int GetMaxRollsAmount()
        {
            return PlayerInfo.MaxRolls;
        }

        public bool IsRegenOn()
        {
            return PlayerInfo.IsRollRegenOn;
        }

        #endregion

        #region Player Points

        public void EarnAbilityPoints(PointsEarnTypes pointsEvent)
        {
            switch (pointsEvent)
            {
                case PointsEarnTypes.LevelUp:
                    PlayerInfo.AbilityPoints += EXTRA_BOSS_POINTS;
                    SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.EarnPointsToast, EXTRA_BOSS_POINTS);
                    break;
                case PointsEarnTypes.BossKill:
                    int points = UnityEngine.Random.Range(1, 4);
                    PlayerInfo.AbilityPoints += points;
                    SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.EarnPointsToast, points);
                    break;
            }

            SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.UpdateAbilityPtsUI, null);
            SDGameLogic.Instance.Player.SavePlayerData();
        }

        public void SpendAbilityPoints(int abilityPoints)
        {
            if (PlayerInfo.AbilityPoints >= abilityPoints)
            {
                PlayerInfo.AbilityPoints -= abilityPoints;
                SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.UpdateAbilityPtsUI, null);
                SDGameLogic.Instance.Player.SavePlayerData();
            }
        }

        public int GetAbilityPointsAmount()
        {
            return PlayerInfo.AbilityPoints;
        }

        public bool CanSpendAbilityPoints(int abilityPoints)
        {
            return PlayerInfo.AbilityPoints >= abilityPoints;
        }

        #endregion
    }

    public enum PointsEarnTypes
    {
        LevelUp,
        BossKill
    }
}
