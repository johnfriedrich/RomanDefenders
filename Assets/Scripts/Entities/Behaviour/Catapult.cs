using System.Collections;
using UnityEngine;

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
        moveAnimLeft.SetBool("move", true);
        moveAnimRight.SetBool("move", true);
    }

    public override void EndMove() {
        base.EndMove();
        moveAnimLeft.SetBool("move", false);
        moveAnimRight.SetBool("move", false);
    }

    public override void Attack(float damage, ParentObject enemy) {
        StartCoroutine(ShootProjectile(damage, enemy));
        baseAnim.SetTrigger("attack");
    }

    private IEnumerator ShootProjectile(float damage, ParentObject enemy) {
        Sound.Instance.PlaySoundClipWithSource(parent.FightSound, parent.AudioSource, 0);
        yield return new WaitForSecondsRealtime(parent.FightSound.SoundDelay);
        GameObject projectile = PoolHolder.Instance.GetObject(projectileType);
        projectile.transform.position = root.transform.position;
        projectile.SetActive(true);
        projectile.GetComponent<Stone>().Shoot(transform, damage, enemy);
    }

}
