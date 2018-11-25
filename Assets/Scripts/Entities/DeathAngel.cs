using System.Collections;
using UnityEngine;

public class DeathAngel : MonoBehaviour {

    [SerializeField]
    private AudioSource audioSource;
    private SoundHolder deathSoundHolder;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private SpriteRenderer spriteRendererHead;
    [SerializeField]
    private SpriteRenderer spriteRendererTeamcolor;

    public SoundHolder DeathSoundHolder {
        get {
            return deathSoundHolder;
        }
    }

    public void Activate(Color teamcolor, SoundHolder deathSoundHolder, Vector3 pos) {
        spriteRendererTeamcolor.color = teamcolor;
        this.deathSoundHolder = deathSoundHolder;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    private void OnEnable() {
        HandleDeath();
    }

    private void HandleDeath() {
        spriteRendererHead.enabled = true;
        spriteRendererTeamcolor.enabled = true;
        anim.SetTrigger("die");
        PlayDeathSound();
        StartCoroutine(DelayRendererDisable());
        StartCoroutine(DelayDisable());
    }

    private void PlayDeathSound() {
        Sound.Instance.PlaySoundClipWithSource(deathSoundHolder, audioSource, 0);
    }

    private IEnumerator DelayRendererDisable() {
        yield return new WaitForSecondsRealtime(0.5f);
        spriteRendererHead.enabled = false;
        spriteRendererTeamcolor.enabled = false;
    }

    private IEnumerator DelayDisable() {
        yield return new WaitForSecondsRealtime(3f);
        gameObject.SetActive(false);
    }

}
