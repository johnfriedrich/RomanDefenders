using System.Collections;
using UnityEngine;

public class AmbientSound : MonoBehaviour {

    [SerializeField]
    private AudioSource ambientSource;
    [SerializeField]
    private SoundHolder[] ambientSounds;
    [SerializeField]
    private float minSeconds;
    [SerializeField]
    private float maxSeconds;

	void Start () {
        StartCoroutine(PlayAmbient());
	}

    private IEnumerator PlayAmbient() {
        float cooldown = Random.Range(minSeconds, maxSeconds);
        yield return new WaitForSecondsRealtime(cooldown);
        int randomSound = Random.Range(0, ambientSounds.Length - 1);
        Debug.Log("played ambient");
        Sound.Instance.PlaySoundClipWithSource(ambientSounds[randomSound], ambientSource, 0);
        StartCoroutine(PlayAmbient());
    }
	
}
