using Manager;
using UnityEngine;

public class StructureHandler : MonoBehaviour {

    private GameObject enemyBase;
    [SerializeField]
    private GameObject basePrefab;
    [SerializeField]
    private GameObject gameParent;

	private void Start () {
        EventManager.Instance.OnStartGamePostEvent += SpawnBase;
	}

    private void SpawnBase() {
        if (enemyBase != null) {
            Destroy(enemyBase);
        }
        enemyBase = Instantiate(basePrefab, gameParent.transform);
    }

}
