using System.Collections;
using Manager;
using Translation;
using UI.HUD;
using UnityEngine;
using LogType = UI.HUD.LogType;

namespace Buildings.Behaviour {
    public class Forge : BuildingBehaviour {

        private static int armorAmount;
        private static int damageAmount;

        [SerializeField]
        private int researchCost;
        [SerializeField]
        private float armorResearchTime;
        [SerializeField]
        private float damageResearchTime;
        private bool isDoingResearch;

        public int ResearchCost => researchCost;

        public static int ArmorAmount => armorAmount;

        public static int DamageAmount => damageAmount;

        public bool IsDoingResearch => isDoingResearch;

        public override void OnBuildingBuilt(ParentBuilding building) {
            base.OnBuildingBuilt(building);
        }

        public override bool LevelUp() {
            //if (isDoingResearch) {
            //    EventLog.Instance.AddAction(LogType.Error, Messages.CantUpgradeWhileResearching, transform.position);
            //    return false;
            //}
            return base.LevelUp();
        }

        public bool ResearchDamage(ButtonCooldown buttonCooldown) {
            bool canReasearch = false;
            if (!Manager.Manager.Instance.HasEnoughMana(researchCost)) {
                EventLog.Instance.AddAction(LogType.Error, "Not enough Mana ", transform.position);
                return canReasearch;
            }

            if (damageAmount == 3) {
                EventLog.Instance.AddAction(LogType.Error, "You already reached max research level", transform.position);
                return canReasearch;
            }
            if (damageAmount == building.CurrentLevel) {
                EventLog.Instance.AddAction(LogType.Error, "Upgrade your Forge ", transform.position);
                return canReasearch;
            }
            if (building.IsUpgrading) {
                EventLog.Instance.AddAction(LogType.Error, "You can't reasearch while the building is upgrading", transform.position);
                return canReasearch;
            }
            if (IsDoingResearch) {
                EventLog.Instance.AddAction(LogType.Error, "You can't research while doing another research", transform.position);
                return canReasearch;
            }

            canReasearch = true;
            buttonCooldown.SetCooldown(damageResearchTime);
            EventManager.Instance.ResearchStarted(this);
            StartCoroutine(UpgradeDamage(damageResearchTime));
            return canReasearch;
        }

        public bool ResearchArmor(ButtonCooldown buttonCooldown) {
            bool canReasearch = false;
            if (!Manager.Manager.Instance.HasEnoughMana(researchCost)) {
                EventLog.Instance.AddAction(LogType.Error, "Not enough Mana ", transform.position);
                return canReasearch;
            }

            if (armorAmount == 3) {
                EventLog.Instance.AddAction(LogType.Error, Messages.AlreadyReachedMaxResearchLevel, transform.position);
                return canReasearch;
            }
            if (armorAmount == building.CurrentLevel) {
                EventLog.Instance.AddAction(LogType.Error, Messages.UpgradeForgeBeforeResearching, transform.position);
                return canReasearch;
            }
            if (building.IsUpgrading) {
                EventLog.Instance.AddAction(LogType.Error, Messages.CantResearchWhileUpgrading, transform.position);
                return canReasearch;
            }
            if (IsDoingResearch) {
                EventLog.Instance.AddAction(LogType.Error, "cant research while doing another research", transform.position);
                return canReasearch;
            }

            canReasearch = true;
            buttonCooldown.SetCooldown(armorResearchTime);
            EventManager.Instance.ResearchStarted(this);
            StartCoroutine(UpgradeArmor(armorResearchTime));
            return canReasearch;
        }

        public override bool Destroy() {
            if (isDoingResearch) {
                EventLog.Instance.AddAction(LogType.Error, Messages.CantDestroyBuildingWhileReasearching, transform.position);
                return false;
            }
            return base.Destroy();
        }

        private IEnumerator UpgradeArmor(float setInTime) {
            isDoingResearch = true;
            yield return new WaitForSeconds(setInTime);
            armorAmount++;
            isDoingResearch = false;
            EventLog.Instance.AddAction(LogType.Upgraded, Messages.ArmorUpgradedTo + armorAmount, transform.position);
        }

        private IEnumerator UpgradeDamage(float setInTime) {
            isDoingResearch = true;
            yield return new WaitForSeconds(setInTime);
            damageAmount++;
            isDoingResearch = false;
            EventLog.Instance.AddAction(LogType.Upgraded, Messages.DamageUpgradedTo + damageAmount, transform.position);
        }

    }
}
