using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour {

    [SerializeField]
    private Text foodValue;
    [SerializeField]
    private Text mana;
    [SerializeField]
    private Text waveTimer;
    [SerializeField]
    private Text waveCount;
    [SerializeField]
    private Text waveFinishedMessage;
    private float timer = 0;

    private void Awake() {
        EventManager.Instance.OnBuildingBuiltEvent += RefreshTopBar;
        EventManager.Instance.OnBuildingRemovedEvent += RefreshTopBar;
        EventManager.Instance.OnEntityTrainEvent += RefreshTopBar;
        EventManager.Instance.OnEntityDeathEvent += RefreshTopBar;
        EventManager.Instance.OnRefreshUIEvent += BuildHUD;
        EventManager.Instance.OnWaveStartEvent += AdjustWaveCount;
        EventManager.Instance.OnAllWavesFinishedEvent += EndedWaves;
        EventManager.Instance.OnGameResetPostEvent += Resetobject;
    }

    private void Update() {
        if (WaveManager.Instance.CurrentWave != null && timer > 0.3f) {
            string text;
            if (WaveManager.Instance.CurrentWave.RemainingWarmupTime != 0) {
                text = WaveManager.Instance.CurrentWave.RemainingWarmupTime.ToString();
            } else {
                text = "Incoming!";
            }
            waveTimer.text = text;
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    private void AdjustWaveCount() {
        if (Manager.Instance.CurrentWaveNumber <= Manager.Instance.MaxWaveNumber) {
            waveCount.text = Manager.Instance.CurrentWaveNumber.ToString() + Path.AltDirectorySeparatorChar + Manager.Instance.MaxWaveNumber;
        }
    }

    private void Resetobject() {
        waveFinishedMessage.enabled = false;
        waveCount.gameObject.transform.parent.gameObject.SetActive(true);
        waveTimer.gameObject.transform.parent.gameObject.SetActive(true);
    }

    private void EndedWaves() {
        waveFinishedMessage.enabled = true;
        waveCount.gameObject.transform.parent.gameObject.SetActive(false);
        waveTimer.gameObject.transform.parent.gameObject.SetActive(false);
    }

    private void Start() {
        BuildHUD();
    }

    private void BuildHUD() {
        RefreshTopBar(null);
        AdjustWaveCount();
    }

    private void RefreshTopBar(ParentObject parentObject, bool byEnemy) {
        RefreshTopBar(parentObject);
    }

    private void RefreshTopBar(ParentObject parentObject) {
        foodValue.text = Manager.Instance.CurrentEntityCount.ToString() + Path.AltDirectorySeparatorChar + Manager.Instance.CurrentFood;
        mana.text = Manager.Instance.Mana.ToString();
    }

}
