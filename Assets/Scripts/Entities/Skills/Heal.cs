using UnityEngine;

public class Heal : SkillBehaviour {

    [SerializeField]
    private int healAmount;

    protected override void Impact() {
        base.Impact();
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliderArray.Length; i++) {
            Entity tempEntity = colliderArray[i].GetComponent<Entity>();
            if (tempEntity != null && tempEntity.IsPlayer() && tempEntity.IsAlive()) {
                tempEntity.RepairSelf(healAmount);
            }
        }
    }

}
