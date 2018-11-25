using UnityEngine;

public class Mainhouse : BuildingBehaviour {

    [SerializeField]
    private GameObject spawnPoint;
    private float costFactor = 1;

    public override void OnBuildingBuilt(ParentBuilding building) {
        base.OnBuildingBuilt(building);
        if (this.building.IsPlayer()) {
            SpawnDefaults();
        }
    }

    public override bool LevelUp() {
        costFactor -= 0.25f;
        return base.LevelUp();
    }

    public bool ReviveHero() {
         return SpawnHero(false);
    }

    public override void OnBuildingRemoved(ParentBuilding building) {
        if (building.IsPlayer()) {
            EventManager.Instance.GameOver();
        } else {
            EventManager.Instance.WinGame();
        }
    }

    private bool SpawnHero(bool firstSpawn) {
        bool canBeSpawned = false;
        Character hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>();
        hero.Load(CharacterEditor.Instance.CharacterSaveData);
        if (!firstSpawn) {
            if (!Manager.Instance.HasEnoughMana(hero.RevivalCost * costFactor)) {
                EventLog.Instance.AddAction(LogType.Error, "You need more Mana!", transform.position);
                return canBeSpawned;
            } else if (hero.IsAlive()) {
                EventLog.Instance.AddAction(LogType.Error, "Hero isn't dead!", transform.position);
                return canBeSpawned;
            }
            ActionText.Instance.SetActionText("Your Hero respawned!", 2f);
            EventLog.Instance.AddAction(LogType.EntityFinished, "Your Hero respawned!", transform.position);
            hero.gameObject.transform.position = spawnPoint.transform.position;
            hero.gameObject.SetActive(true);
            return !canBeSpawned;
        }
        hero.enabled = true;
        hero.gameObject.transform.position = spawnPoint.transform.position;
        hero.Agent.enabled = true;
        hero.gameObject.SetActive(true);
        return !canBeSpawned;
    }

    private void SpawnDefaults() {
        if (building.IsPlayer()) {
            Entity entityToTrain;
            for (int i = 0; i < 3; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Swordsman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);
            }
            for (int i = 0; i < 3; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Bowman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);

            }
            for (int i = 0; i < 1; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Horseman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);

            }
            SpawnHero(true);
        }
    }

    private void SpawnHelper(Entity entity) {
        entity.gameObject.transform.position = spawnPoint.transform.position;
        entity.SetOwner(OwnerEnum.Player);
        entity.gameObject.SetActive(true);
    }

    private void OnEnable() {
        costFactor = 1;
    }

}
