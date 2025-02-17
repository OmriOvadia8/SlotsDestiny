using UnityEngine;
using SD_GameLoad;
using SD_Core;
using UnityEngine.UI;
using SD_Boss;
using SD_UI;
using SD_Ability;

namespace SD_Test
{
#if UNITY_EDITOR
    /// <summary>
    /// A class for conducting various testing operations within the Unity editor.
    /// </summary>
    public class SDTest : SDLogicMonoBehaviour
    {
        [SerializeField] Image healthBar;
        [SerializeField] SDBossAnimationsController bossAnim;
        [SerializeField] SDToastingManager toasting;
        [SerializeField] SDAbilityAnimationController abilityAnim;
        [SerializeField] double XPCheat;
        public double currentHealth = 1000;
        public double maxHealth = 1000;

        private void Start()
        {
            SDDebug.Log(CurrentBossInfo.Level);
            SDDebug.Log(CurrentBossInfo.CurrentHp);
            SDDebug.Log(CurrentBossInfo.TotalHp);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameLogic.BossController.DamageBoss(100);
                SDDebug.Log(CurrentBossInfo.CurrentHp);
                SDDebug.Log(CurrentBossInfo.Level);
                SDDebug.Log(CurrentBossInfo.IsAlive);
            }

            if(Input.GetKeyDown(KeyCode.C))
            {
                bossAnim.HurtBoss();
                TakeDamage(CurrentBossInfo.CurrentHp - (CurrentBossInfo.CurrentHp -1));
                SDDebug.Log("index" + CurrentBossInfo.Index);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                SDDebug.Log(GameLogic.Player.PlayerData.PlayerInfo.Level);
                SDDebug.Log(GameLogic.Player.PlayerData.PlayerInfo.TotalXpRequired);
                SDDebug.Log(GameLogic.Player.PlayerData.PlayerInfo.CurrentXp);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                GameLogic.PlayerController.AddPlayerXP(500);
                GameLogic.Player.SavePlayerData();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                GameLogic.PlayerController.EarnAbilityPoints(PointsEarnTypes.BossKill);
                SDManager.Instance.EventsManager.InvokeEvent(SDEventNames.CheckUnlockAbility, GameLogic.Player.PlayerData.PlayerInfo.Level);
                GameLogic.Player.SavePlayerData();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                toasting.DisplayTextToast(10, PoolNames.XPToast);
                toasting.DisplayTextToast(10, PoolNames.EarnPointsToast);
                toasting.DisplayTextToast(10, PoolNames.SpendPointsToast);
                toasting.DisplayTextToast(10, PoolNames.LevelUpToast);
            }

            if(Input.GetKeyDown(KeyCode.R))
            {
                GameLogic.PlayerController.IncreaseRoll(10);
                InvokeEvent(SDEventNames.FailAdToast, null);
            }

            if(Input.GetKeyDown(KeyCode.F1))
            {
                abilityAnim.UseAbility("SkullSmoke", 1);
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                abilityAnim.UseAbility("Slashes", 1);
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                abilityAnim.UseAbility("Scratch", 1);
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                abilityAnim.UseAbility("SmokeExplosion", 1);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                abilityAnim.UseAbility("SkullExplosion", 1);
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                abilityAnim.UseAbility("Tornado", 1);
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                abilityAnim.UseAbility("Tentacle", 1);
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                GameLogic.PlayerController.AddPlayerXP(XPCheat);
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                GameLogic.PlayerController.IncreaseRoll(30);
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                GameLogic.BossController.DamageBoss(30000);
            }
        }

        public void TakeDamage(double damage)
        {
            currentHealth -= damage;
            UpdateHealthBar(currentHealth, maxHealth);
        }

        void UpdateHealthBar(double currentHealth, double maxHealth)
        {
            float fillAmount = (float)(currentHealth / maxHealth);
            healthBar.fillAmount = fillAmount;
        }
    }
#endif
}
