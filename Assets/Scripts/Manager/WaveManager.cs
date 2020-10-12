using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    public static WaveManager Instance { get; private set; }

    public int CurrentWaveNumber { get; internal set; } = 0;

    [SerializeField]
    private Wave[] waves;
    private Wave currentWave;

    public Wave CurrentWave {
        get {
            return currentWave;
        }
    }

    private void Awake() {
        Instance = this;
        Manager.Instance.MaxWaveNumber = waves.Length;
    }

    private void Start() {
        EventManager.Instance.OnStartGamePostEvent += NextWave;
        EventManager.Instance.OnWaveFinishEvent += NextWave;
        EventManager.Instance.OnGameResetPreEvent += ResetObject;
    }

    private void ResetObject() {
        StopAllCoroutines();
        if (currentWave != null) {
            Destroy(currentWave.gameObject);
        }
        CurrentWaveNumber = 0;
    }

    private void NextWave() {
        if (currentWave != null && currentWave.gameObject != null) {
            Destroy(currentWave.gameObject);
        }
        if (CurrentWaveNumber == waves.Length) {
            Sound.Instance.PlaySoundClip(SoundEnum.UI_Pling);
            EventManager.Instance.AllWavesFinished();
            return;
        }
        currentWave = Instantiate(waves[CurrentWaveNumber]);
        StartCoroutine(WaveSpawner());
    }

    private IEnumerator WaveSpawner() {
        yield return new WaitForSeconds(currentWave.WarmupTime);
        Spawn();
    }

    private void Spawn() {
        currentWave.StartWave();
        CurrentWaveNumber++;
        EventManager.Instance.WaveStart();
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Pling);
        ActionText.Instance.SetActionText("Next Wave is coming! Be ready!", 2f);
    }

}
