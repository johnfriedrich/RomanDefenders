using System.Collections;
using ObjectManagement;
using Parent;
using Projectiles;
using UnityEngine;

namespace Entities.Behaviour {
    public class Horseman : EntityBehaviour {

        [SerializeField]
        private Animator horseAnim;
        [SerializeField]
        private Animator riderAnim;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private ParentObjectNameEnum projectileType;

        public override void Attack(float damage, ParentObject enemy) {
            riderAnim.SetTrigger(AnimEnum.Attack);
            StartCoroutine(ShootProjectile(damage, enemy));
        }

        private IEnumerator ShootProjectile(float damage, ParentObject enemy) {
            yield return new WaitForSeconds(parent.FightSound.SoundDelay);
            Sound.Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
            var projectile = PoolHolder.Instance.GetObject(projectileType);
            projectile.transform.position = root.transform.position;
            projectile.SetActive(true);
            projectile.GetComponent<Arrow>().Shoot(transform, damage, enemy);
        }

        public override void StartMove() {
            riderAnim.SetBool(AnimEnum.Move, true);
            horseAnim.SetBool(AnimEnum.Move, true);
        }

        public override void EndMove() {
            riderAnim.SetBool(AnimEnum.Move, false);
            horseAnim.SetBool(AnimEnum.Move, false);
        }

    }
}
