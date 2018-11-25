using UnityEngine;
using UnityEngine.UI;

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
        } else if (hero.Casting) {
            ActionText.Instance.SetActionText(hero.CharacterName + " is already casting!", 1);
            return;
        }
        Manager.Instance.SkillOnMouse = true;
        targeter.gameObject.SetActive(true);
        targeter.CurrentSkillButton = this;
        targeter.CurrentSkill = skillObject.GetComponent<SkillBehaviour>();
        targeter.enabled = true;
    }

    public void ExecuteSkill(Transform target) {
        Manager.Instance.SkillOnMouse = false;
        PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>().Cast(skillObject, target);
        buttonCooldown.SetCooldown(skill.CooldownAmount);
    }

    private void Reset(SkillBehaviour canceledSkill) {
        if (canceledSkill.Name == skill.Name) {
            buttonCooldown.Finish();
        }
    }

    private void Start() {
        targeter = Manager.Instance.SkillTargeter.GetComponent<SkillTargeter>();
        skill = skillObject.GetComponent<SkillBehaviour>();
        uiSprite.sprite = skill.UiSprite;
        EventManager.Instance.OnCancelSkillEvent += Reset;
    }

}
