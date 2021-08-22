using System.Collections.Generic;
using Buildings.Behaviour;
using Buildings.Walls;
using Entities;
using Manager;
using ObjectManagement;
using Parent;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.HUD {
    public class Commands : Menu {

        [SerializeField]
        private Button setPassive;
        [SerializeField]
        private Button setAgressive;
        [SerializeField]
        private Button trainCatapult;
        [SerializeField]
        private Button trainSwordsman;
        [SerializeField]
        private Button trainBowman;
        [SerializeField]
        private Button trainHorseman;
        [SerializeField]
        private Button trainSettler;
        [SerializeField]
        private Button researchArmor;
        [SerializeField]
        private ButtonCooldown researchArmorCooldown;
        [SerializeField]
        private Button researchDamage;
        [SerializeField]
        private ButtonCooldown researchDamageCooldown;
        [SerializeField]
        private Button addEntity;
        [SerializeField]
        private Button dropEntity;
        [SerializeField]
        private Button destroy;
        [SerializeField]
        private Button upgrade;
        [SerializeField]
        private Button reviveHero;
        private ButtonCooldown upgradeCooldown;
        [SerializeField]
        private Text upgradeCooldownText;
        [SerializeField]
        private Image upgradeCooldownImage;
        [SerializeField]
        private CommandTooltip commandTooltip;
        private static Commands _instance;
        private List<ParentObject> selectedObjects;

        public static Commands Instance => _instance;

        public List<ParentObject> SelectedObjects => selectedObjects;

        public void EndHover() {
            commandTooltip.Hide();
        }

        public void HoverPassive() {
            commandTooltip.SetTooltip(new TooltipData("Passive", "When in passive mode, the entity does not attack. Event when he gets damage.", null, 0, 0));
        }

        public void SetPassive() {
            foreach (Entity entity in selectedObjects) {
                entity.SetPassive();
            }
            PlayButtonClick();
        }

        public void HoverAgressive() {
            commandTooltip.SetTooltip(new TooltipData("Agressive", "When in agressive mode, the entity does attack every close enemy.", null, 0, 0));
        }

        public void SetAgressive() {
            foreach (Entity entity in selectedObjects) {
                entity.SetAgressive();
            }
            PlayButtonClick();
        }

        public void HoverReviveHero() {
            Character hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>();
            commandTooltip.SetTooltip(new TooltipData("Revive Hero", "This revives your hero if he died.", Manager.Manager.Instance.ManaSprite, hero.RevivalCost, 0));
        }

        public void ReviveHero() {
            ParentBuilding temp = (ParentBuilding)selectedObjects[0];
            Mainhouse tempBehaviour = (Mainhouse)temp.Behaviour;
            tempBehaviour.ReviveHero();
            PlayButtonClick();
        }

        public void HoverUpgrade() {
            var building = selectedObjects[0].GetComponent<ParentBuilding>();
            commandTooltip.SetTooltip(new TooltipData("Upgrade Building", building.UpgradeDescription, Manager.Manager.Instance.ManaSprite, building.UpgradeCost, 0));
        }

        public void Upgrade() {
            var building = selectedObjects[0].GetComponent<ParentBuilding>();
            if (building.LevelUp()) {
                PlayButtonClick();
            }
        }

        public void HoverTrainUnit(TrainUnit unit) {
            var entity = (Entity)PrefabHolder.Instance.GetInfo(unit.entityType);
            commandTooltip.SetTooltip(new TooltipData("Trains a " + entity.FriendlyName, "This trains a " + entity.FriendlyName + " in this barracks.", Manager.Manager.Instance.ManaSprite, entity.BuildCost, 0));
        }

        public void TrainUnit(TrainUnit unit) {
            ParentBuilding building = (ParentBuilding)selectedObjects[0];
            Barracks barracks = (Barracks)building.Behaviour;
            if (barracks.Train(unit)) {
                PlayButtonClick();
            }
        }

        public void HoverTrainSettler(TrainUnit unit) {
            var entity = (Entity)PrefabHolder.Instance.GetInfo(unit.entityType);
            commandTooltip.SetTooltip(new TooltipData("Trains a " + entity.FriendlyName, "This trains a " + entity.FriendlyName + " in Mainhouse.", Manager.Manager.Instance.ManaSprite, entity.BuildCost, 0));
        }

        public void TrainSettler(TrainUnit unit) {
            var building = (ParentBuilding)selectedObjects[0];
            var mainhouse = (Mainhouse)building.Behaviour;
            if (mainhouse.Train(unit)) {
                PlayButtonClick();
            }
        }

        public void HoverResearchArmor() {
            var forge = (Forge)selectedObjects[0].GetComponent<ParentBuilding>().Behaviour;
            commandTooltip.SetTooltip(new TooltipData("Research Armor", "This researches higher armor level for your soldiers of not already at max Level.", Manager.Manager.Instance.ManaSprite, forge.ResearchCost, 0));
        }

        public void ResearchArmor() {
            if (researchArmorCooldown.IsFinished()) {
                ParentBuilding building = (ParentBuilding)selectedObjects[0];
                Forge forge = (Forge)building.Behaviour;
                if (forge.ResearchArmor(researchArmorCooldown)) {
                    PlayButtonClick();
                }
                return;
            }
            Debug.Log("Please wait " + researchArmorCooldown.RemainingCooldown + " seconds");
        }

        public void HoverResearchDamage() {
            var forge = (Forge)selectedObjects[0].GetComponent<ParentBuilding>().Behaviour;
            commandTooltip.SetTooltip(new TooltipData("Research Damage", "This researches higher damage level for your soldiers of not already at max Level.", Manager.Manager.Instance.ManaSprite, forge.ResearchCost, 0));
        }

        public void ResearchDamage() {
            if (researchDamageCooldown.IsFinished()) {
                var building = (ParentBuilding)selectedObjects[0];
                var forge = (Forge)building.Behaviour;
                if (forge.ResearchDamage(researchDamageCooldown)) {
                    PlayButtonClick();
                }
                return;
            }
            Debug.Log("Please wait " + researchDamageCooldown.RemainingCooldown + " seconds");
        }

        public void HoverAddEntity() {
            var wall = (Wall)selectedObjects[0].GetComponent<ParentBuilding>().Behaviour;
            commandTooltip.SetTooltip(new TooltipData("Put a Bowman on Wall", "This puts a near Bowman on this Wall if available.", null, 0, 0));
        }

        public void AddEntity() {
            var wall = selectedObjects[0].GetComponent<Wall>();
            if (wall.AddEntity(EntityUtils.GetNearestEntityByType(20, ParentObjectNameEnum.Bowman, wall.gameObject.transform))) {
                PlayButtonClick();
            }
        }

        public void HoverDestroy() {
            commandTooltip.SetTooltip(new TooltipData("Destroy Building", "This destroys the building. You get half the amount of the initial build costs back", Manager.Manager.Instance.ManaSprite, 0, 0));
        }

        public void Destroy() {
            PlayButtonClick();
            selectedObjects[0].GetComponent<ParentBuilding>().Destroy(false);
        }

        public void HoverDropEntity() {
            var wall = (Wall)selectedObjects[0].GetComponent<ParentBuilding>().Behaviour;
            commandTooltip.SetTooltip(new TooltipData("Drop Bowman from Wall", "This drops a Bowman from this Wall.", null, 0, 0));
        }

        public void DropEntity() {
            var wall = selectedObjects[0].GetComponent<Wall>();
            if (wall.DropEntity(true)) {
                PlayButtonClick();
            }
        }

        private void SetSelectedObjects(List<ParentObject> parentObjects) {
            Clear();
            selectedObjects = parentObjects;
            if (selectedObjects[0] is Entity) {
                ShowAsEntity();
                Show();
            } else {
                ShowAsBuilding();
                var building = (ParentBuilding)selectedObjects[0];
                if (building.Behaviour is Barracks) {
                    ShowAsBarracks();
                } else if (building.Behaviour is Forge) {
                    ShowAsForge();
                } else if (building.Behaviour is Wall) {
                    ShowAsWall();
                } else if (building.Behaviour is Mainhouse) {
                    ShowAsMainhouse();
                }
                Show();
            }
        }

        private void Awake() {
            _instance = this;
        }

        private void Start() {
            selectedObjects = new List<ParentObject>();
            EventManager.Instance.OnParentObjectSelectedEvent += SetSelectedObjects;
            EventManager.Instance.OnDeselectEvent += Deselect;
            EventManager.Instance.OnEntityDeathEvent += Disable;
            EventManager.Instance.OnBuildingRemovedEvent += Disable;
            Hide();
        }

        private void PlayButtonClick() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        }

        private void ShowAsMainhouse() {
            reviveHero.gameObject.SetActive(true);
            trainSettler.gameObject.SetActive(true);
            destroy.gameObject.SetActive(false);
        }

        private void Deselect() {
            Clear();
            selectedObjects = new List<ParentObject>();
            Hide();
        }

        private void Clear() {
            if (selectedObjects != null && selectedObjects.Count > 0) {
                foreach (ParentObject pObject in selectedObjects) {
                    if (pObject != null) {
                        pObject.InfoBar.Hide();
                        pObject.HideCircle();
                    }
                }
            }
            setPassive.gameObject.SetActive(false);
            setAgressive.gameObject.SetActive(false);
            destroy.gameObject.SetActive(false);
            upgrade.gameObject.SetActive(false);
            reviveHero.gameObject.SetActive(false);
            trainCatapult.gameObject.SetActive(false);
            trainSwordsman.gameObject.SetActive(false);
            trainBowman.gameObject.SetActive(false);
            trainHorseman.gameObject.SetActive(false);
            researchArmor.gameObject.SetActive(false);
            researchDamage.gameObject.SetActive(false);
            addEntity.gameObject.SetActive(false);
            dropEntity.gameObject.SetActive(false);
            trainSettler.gameObject.SetActive(false);
        }

        private void ShowAsEntity() {
            setPassive.gameObject.SetActive(true);
            setAgressive.gameObject.SetActive(true);
            upgrade.gameObject.SetActive(false);
        }

        private void ShowAsBuilding() {
            destroy.gameObject.SetActive(true);
            var temp = (ParentBuilding)selectedObjects[0];
            if (upgradeCooldown != null) {
                upgradeCooldown.Clear();
            }
            upgradeCooldown = temp.UpgradeCooldown;
            upgradeCooldown.SetupCooldown(upgradeCooldownText, upgradeCooldownImage);
            if (!temp.IsMaxLevel()) {
                upgrade.gameObject.SetActive(true);
            }
        }

        private void ShowAsBarracks() {
            trainCatapult.gameObject.SetActive(true);
            trainSwordsman.gameObject.SetActive(true);
            trainBowman.gameObject.SetActive(true);
            trainHorseman.gameObject.SetActive(true);
        }

        private void ShowAsForge() {
            researchArmor.gameObject.SetActive(true);
            researchDamage.gameObject.SetActive(true);
        }

        private void ShowAsWall() {
            addEntity.gameObject.SetActive(true);
            dropEntity.gameObject.SetActive(true);
        }

        private void Disable(ParentObject deadObject) {
            if (selectedObjects != null) {
                if (selectedObjects.Count == 0) {
                    return;
                }
                if (deadObject == selectedObjects[0]) {
                    Clear();
                    selectedObjects[0] = null;
                    Hide();
                }
            }
        }

        private void Disable(ParentObject deadObject, bool byEnemy) {
            Disable(deadObject);
        }

        private Vector3 RandomizePosition(Vector3 oPos, float range) {
            var offsetX = Random.Range(1f, range);
            var offsetZ = Random.Range(1f, range);
            return new Vector3(oPos.x + offsetX, oPos.y, oPos.z + offsetZ);
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.Mouse1) || selectedObjects == null || !(selectedObjects[0] is Entity)) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            foreach (Entity entity in selectedObjects) {
                if (entity.IsAlive() && !Manager.Manager.Instance.SkillOnMouse) {
                    if (selectedObjects.Count > 15) {
                        entity.MoveTo(RandomizePosition(hit.point, 8), false);
                        continue;
                    }

                    if (selectedObjects.Count > 4) {
                        entity.MoveTo(RandomizePosition(hit.point, 5), false);
                        continue;
                    }

                    entity.MoveTo(hit.point, false);
                }
            }
        }

    }
}
