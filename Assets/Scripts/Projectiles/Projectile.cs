using Parent;
using UnityEngine;

namespace Projectiles {
    public class Projectile : MonoBehaviour {

        [SerializeField]
        protected float speed;
        [SerializeField]
        protected Vector3 backupTarget;

        public virtual void Shoot(Transform shooter, float damage, ParentObject enemy) {
            backupTarget = enemy.GetHitpoint(transform.position);
        }
    }
}
