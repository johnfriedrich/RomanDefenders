using System.Collections;
using Entities.Skills;
using ObjectManagement;
using Parent;
using UnityEngine;

namespace Entities.Behaviour {
    public class CharacterBehaviour : EntityBehaviour {

        [SerializeField]
        private GameObject skillRoot;
        [SerializeField]
        private GameObject idleSkillRoot;
        private float timer = 0;
        private SkillBehaviour currentSkill;
        [SerializeField]
        private GameObject testSkill;
        private Character hero;
        private IEnumerator lookAtCoroutine;
        private IEnumerator switchEffectCoroutine;
        private IEnumerator resetCastCoroutine;

        public override void CancelCast() {
            base.CancelCast();
            StopCoroutine(lookAtCoroutine);
            StopCoroutine(switchEffectCoroutine);
            StopCoroutine(resetCastCoroutine);
            currentSkill.GetComponent<SkillBehaviour>().Cancel();
        }

        public override void Cast(GameObject skill, Transform target) {
            base.Cast(skill, target);
            transform.LookAt(target);
            currentSkill = Instantiate(skill, skillRoot.transform.position, Quaternion.identity).GetComponent<SkillBehaviour>();
            currentSkill.gameObject.transform.SetParent(skillRoot.transform, false);
            lookAtCoroutine = LookAt(currentSkill, target);
            StartCoroutine(lookAtCoroutine);
            currentSkill.gameObject.transform.position = skillRoot.transform.position;
            switchEffectCoroutine = SwitchEffect(currentSkill);
            StartCoroutine(switchEffectCoroutine);
            resetCastCoroutine = DelaySkill(currentSkill, target);
            StartCoroutine(resetCastCoroutine);
        }

        public override void Attack(float damage, ParentObject enemy) {
            if (hero == null) {
                hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>();
            }
            if (hero.Casting) {
                return;
            }
            hero.Cast(testSkill, enemy.gameObject.transform);
            Debug.Log("attack");
        }

        private IEnumerator LookAt(SkillBehaviour skill, Transform target) {
            while (skill.gameObject.transform.parent != null) {
                skill.gameObject.transform.LookAt(target);
                yield return null;
            }
        }

        private IEnumerator SwitchEffect(SkillBehaviour skill) {
            yield return new WaitForSeconds(skill.CastTime * 0.8f);
            skill.BigEffect.SetActive(true);
        }

        private IEnumerator DelaySkill(SkillBehaviour skill, Transform target) {
            yield return new WaitForSeconds(skill.CastTime);
            skill.SmallEffect.SetActive(false);
            skill.gameObject.transform.parent = null;
            skill.SetDestination(target, parent);
        }

        private void PlayCustomIdle() {
            baseAnim.SetTrigger("idle_play");
        }

        private void OnEnable() {
            StopAllCoroutines();
            if (currentSkill != null && currentSkill.gameObject != null) {
                Destroy(currentSkill.gameObject);
            }
        }

        private void Update() {
            timer += Time.deltaTime;
            if (timer > 25 && parent.IsIdle()) {
                PlayCustomIdle();
                timer = 0;
            }
        }

    }
}
