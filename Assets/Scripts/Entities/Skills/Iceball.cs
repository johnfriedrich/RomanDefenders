using UnityEngine;

namespace Entities.Skills {
    public class Iceball : SkillBehaviour {

        protected override void Impact() {
            base.Impact();
            var colliderArray = Physics.OverlapSphere(transform.position, radius);
            foreach (var t in colliderArray) {
                var tempEntity = t.GetComponent<Entity>();
                if (tempEntity != null && !tempEntity.IsPlayer() && tempEntity.IsAlive()) {
                    tempEntity.TakeDamage(damage);
                }
            }
        }

    }
}
