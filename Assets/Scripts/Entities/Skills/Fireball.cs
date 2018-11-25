using UnityEngine;

public class Fireball : SkillBehaviour {

    protected override void Impact() {
        base.Impact();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliderArray.Length; i++) {
            Entity tempEntity = colliderArray[i].GetComponent<Entity>();
            if (tempEntity != null && !tempEntity.IsPlayer() && tempEntity.IsAlive()) {
                tempEntity.TakeDamage(damage);
            }
        }
    }

}
