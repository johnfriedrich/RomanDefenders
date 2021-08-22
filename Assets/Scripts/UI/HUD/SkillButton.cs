using Entities;
using Entities.Skills;
using Manager;
using ObjectManagement;
using Parent;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class SkillButton : MonoBehaviour {

        [SerializeField]
        private ButtonCooldown buttonCooldown;
        [SerializeField]
        private GameObject skillObject;
        private SkillBehaviour skill;
        [SerializeField]
        private CommandTooltip commandTooltip;
        [SerializeField]
        private Image uiSprite;
        private SkillTargeter targeter;
        private Character hero;

        public void HoverSkill() {
            commandTooltip.SetTooltip(new TooltipData(skill.Name, skill.Description, null, 0, skill.CooldownAmount));
        }

        public void EndHover() {
            commandTooltip.gameObject.SetActive(false);
        }

        public void PlaceSkill() {
            if (hero == null) {
                hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>();
            }
            if (!buttonCooldown.IsFinished()) {
                ActionText.Instance.SetActionText(skill.Name + " is still on cooldown!", 1);
                return;
            }

            if (hero.Casting) {
                ActionText.Instance.SetActionText(hero.CharacterName + " is already casting!", 1);
                return;
            }
            Manager.Manager.Instance.SkillOnMouse = true;
            targeter.SetSkill(skillObject.GetComponent<SkillBehaviour>(), this);
        }

        public void ExecuteSkill(Transform target) {
            Manager.Manager.Instance.SkillOnMouse = false;
            PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>().Cast(skillObject, target);
            buttonCooldown.SetCooldown(skill.CooldownAmount);
        }

        private void Reset(SkillBehaviour canceledSkill) {
            if (canceledSkill.Name == skill.Name) {
                buttonCooldown.Finish();
            }
        }

        private void Start() {
            targeter = Manager.Manager.Instance.SkillTargeter.GetComponent<SkillTargeter>();
            skill = skillObject.GetComponent<SkillBehaviour>();
            uiSprite.sprite = skill.UiSprite;
            EventManager.Instance.OnCancelSkillEvent += Reset;
        }

    }
}
