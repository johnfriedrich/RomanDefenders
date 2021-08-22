using System.Collections;
using Parent;
using UnityEngine;

namespace Projectiles {
    public class Arrow : Projectile {

        public override void Shoot(Transform shooter, float damage, ParentObject enemy) {
            base.Shoot(shooter, damage, enemy);
            StartCoroutine(MoveUntilDone(shooter, damage, enemy));
        }

        private IEnumerator MoveUntilDone(Transform shooter, float damage, ParentObject enemy) {
            transform.LookAt(enemy.GetHitpoint(transform.position));
            while (enemy != null && Vector3.Distance(transform.position, enemy.GetHitpoint(transform.position)) > 0.1f) {
                transform.LookAt(enemy.GetHitpoint(transform.position));
                transform.position = Vector3.MoveTowards(transform.position, enemy.GetHitpoint(transform.position), speed * Time.deltaTime);
                yield return null;
            }
            //Enemy could be dead by now
            if (enemy != null && enemy.isActiveAndEnabled) {
                enemy.TakeDamage(damage);
                gameObject.SetActive(false);
            } else {
                while (Vector3.Distance(transform.position, backupTarget) > 0.1f) {
                    transform.LookAt(backupTarget);
                    transform.position = Vector3.MoveTowards(transform.position, backupTarget, speed * Time.deltaTime);
                    yield return null;
                }
                gameObject.SetActive(false);
            }
        }

    }
}
