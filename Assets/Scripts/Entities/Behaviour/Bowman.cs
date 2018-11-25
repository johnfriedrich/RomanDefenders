using System.Collections;
using UnityEngine;

public class Bowman : EntityBehaviour {

    [SerializeField]
    private ParentObjectNameEnum projectileType;
    [SerializeField]
    private GameObject root;
    private float timer = 0;

    public override void Attack(float damage, ParentObject enemy) {
        StartCoroutine(ShootProjectile(parent.CurrentEnemy, damage, enemy));
        baseAnim.SetTrigger("attack");
    }

    private IEnumerator ShootProjectile(GameObject destination, float damage, ParentObject enemy) {
        yield return new WaitForSeconds(parent.FightSound.SoundDelay);
        Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
        GameObject projectile = PoolHolder.Instance.GetObject(projectileType);
        projectile.transform.position = root.transform.position;
        projectile.SetActive(true);
        projectile.GetComponent<Arrow>().Shoot(transform, damage, enemy);
    }

    private void PlayCustomIdle() {
        if (Random.Range(0, 2) == 1) {
            baseAnim.SetTrigger("idle_kick");
        } else {
            baseAnim.SetTrigger("idle_head");
        }
    }

    private void Update() {
        timer += Time.deltaTime;
        if (timer > 15 && parent.IsIdle()) {
            PlayCustomIdle();
            timer = 0;
        }
    }

}
