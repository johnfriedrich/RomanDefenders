using System.Collections;
using UnityEngine;

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
        riderAnim.SetTrigger("attack");
        StartCoroutine(ShootProjectile(damage, enemy));
    }

    private IEnumerator ShootProjectile(float damage, ParentObject enemy) {
        yield return new WaitForSeconds(parent.FightSound.SoundDelay);
        Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
        GameObject projectile = PoolHolder.Instance.GetObject(projectileType);
        projectile.transform.position = root.transform.position;
        projectile.SetActive(true);
        projectile.GetComponent<Arrow>().Shoot(transform, damage, enemy);
    }

    public override void StartMove() {
        riderAnim.SetBool("move", true);
        horseAnim.SetBool("move", true);
    }

    public override void EndMove() {
        riderAnim.SetBool("move", false);
        horseAnim.SetBool("move", false);
    }

}
