using System.Collections;
using UnityEngine;

public class DeathObject : MonoBehaviour {

    [SerializeField]
    private AudioSource audioSource;
    private SoundHolder destroySoundHolder;

    public void Activate(SoundHolder destroySoundHolder, Vector3 pos) {
        this.destroySoundHolder = destroySoundHolder;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    private void HandleDeath() {
        PlayDeathSound();
        StartCoroutine(DelayDisable());
    }

    private void OnEnable() {
        HandleDeath();
    }

    private void PlayDeathSound() {
        Sound.Instance.PlaySoundClipWithSource(destroySoundHolder, audioSource, 0);
    }

    private IEnumerator DelayDisable() {
        yield return new WaitForSecondsRealtime(3f);
        gameObject.SetActive(false);
    }

}
