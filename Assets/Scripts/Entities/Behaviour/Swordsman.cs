using System.Collections;
using Parent;
using UnityEngine;

namespace Entities.Behaviour {
    public class Swordsman : EntityBehaviour {

        public override void Attack(float damage, ParentObject enemy) {
            var random = Random.Range(0, 2);
            if (random == 1) {
                baseAnim.SetTrigger("attack_side");
                StartCoroutine(Delay(damage, enemy, 0.65f));
            } else {
                baseAnim.SetTrigger("attack_top");
                StartCoroutine(Delay(damage, enemy, 0.9f));
            }
        }

        private IEnumerator Delay(float damage, ParentObject enemy, float time) {
            //Wait for sword hit
            yield return new WaitForSecondsRealtime(time);
            if (enemy != null && enemy.isActiveAndEnabled) {
                //Enemy could be dead by that time
                enemy.TakeDamage(damage);
                Sound.Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
            }
        }
    }
}
