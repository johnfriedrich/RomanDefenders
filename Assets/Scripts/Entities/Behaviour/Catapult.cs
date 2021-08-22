using System.Collections;
using ObjectManagement;
using Parent;
using Projectiles;
using UnityEngine;

namespace Entities.Behaviour {
    public class Catapult : EntityBehaviour {

        [SerializeField]
        private ParentObjectNameEnum projectileType;
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private Animator moveAnimLeft;
        [SerializeField]
        private Animator moveAnimRight;
        [SerializeField]
        private SkinnedMeshRenderer romanLeft;
        [SerializeField]
        private SkinnedMeshRenderer romanRight;

        public override void SetTeamColor(Color color) {
            base.SetTeamColor(color);
            romanLeft.materials[2].color = color;
            romanRight.materials[2].color = color;
        }

        public override void StartMove() {
            base.StartMove();
            moveAnimLeft.SetBool(AnimEnum.Move, true);
            moveAnimRight.SetBool(AnimEnum.Move, true);
        }

        public override void EndMove() {
            base.EndMove();
            moveAnimLeft.SetBool(AnimEnum.Move, false);
            moveAnimRight.SetBool(AnimEnum.Move, false);
        }

        public override void Attack(float damage, ParentObject enemy) {
            StartCoroutine(ShootProjectile(damage, enemy));
            baseAnim.SetTrigger(AnimEnum.Attack);
        }

        private IEnumerator ShootProjectile(float damage, ParentObject enemy) {
            Sound.Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
            yield return new WaitForSecondsRealtime(parent.FightSound.SoundDelay);
            var projectile = PoolHolder.Instance.GetObject(projectileType);
            projectile.transform.position = root.transform.position;
            projectile.SetActive(true);
            projectile.GetComponent<Stone>().Shoot(transform, damage, enemy);
        }

    }
}
