using Entities.Skills;
using Parent;
using UnityEngine;

namespace Entities.Behaviour {
    public class EntityBehaviour : MonoBehaviour {

        [SerializeField]
        protected Animator baseAnim;
        protected Entity parent;

        private const float DefaultCastSpeed = 5f;

        public virtual void SetTeamColor(Color color) { }

        public virtual void Attack(float damage, ParentObject enemy) {}

        public Entity GetEntity() { return parent; }

        public virtual void CancelCast() {
            baseAnim.gameObject.SetActive(false);
            baseAnim.gameObject.SetActive(true);
        }

        public virtual void Cast(GameObject skill, Transform target) {
            SkillBehaviour skillBehaviour = skill.GetComponent<SkillBehaviour>();
            float multiplier = DefaultCastSpeed / skillBehaviour.CastTime;
            parent.Stop();
            baseAnim.SetFloat("castSpeed", multiplier);
            baseAnim.SetTrigger(AnimEnum.Attack);
        }

        public virtual void StartMove() {
            baseAnim.SetBool(AnimEnum.Move, true);
        }

        public virtual void EndMove() {
            baseAnim.SetBool(AnimEnum.Move, false);
        }

        private void Start() {
            parent = GetComponent<Entity>();
        }

    }
}
