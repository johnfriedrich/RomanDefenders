using System.Collections;
using UnityEngine;

public class Stone : Projectile {

    public override void Shoot(Transform shooter, float damage, ParentObject enemy) {
        base.Shoot(shooter, damage, enemy);
        StartCoroutine(MoveUntilDone(shooter, damage, enemy));
    }

    private IEnumerator MoveUntilDone(Transform shooter, float damage, ParentObject enemy) {
        while (enemy != null && Vector3.Distance(transform.position, enemy.GetHitpoint(transform.position)) > 0.1f) {
            transform.RotateAround(transform.position, Vector3.up, 2);
            transform.position = Vector3.MoveTowards(transform.position, enemy.GetHitpoint(transform.position), speed * Time.deltaTime);
            yield return null;
        }
        //Enemy could be dead by now
        if (enemy != null && enemy.isActiveAndEnabled) {
            enemy.TakeDamage(damage);
            if (enemy is ParentBuilding) {
                ParentBuilding temp = (ParentBuilding)enemy;
                if (temp.gameObject.activeInHierarchy) {
                    temp.SpawnDustParticles(2);
                    temp.Shake();
                }
            }
            gameObject.SetActive(false);
        } else {
            while (Vector3.Distance(transform.position, backupTarget) > 0.1f) {
                transform.RotateAround(transform.position, Vector3.up, 2);
                transform.position = Vector3.MoveTowards(transform.position, backupTarget, speed * Time.deltaTime);
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }

}
