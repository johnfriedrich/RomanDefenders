using UnityEngine;

public class Wall : BuildingBehaviour {

    public static readonly int MaxEntitiesOnWall = 3;

    private GameObject[] entitiesOnWall = new GameObject[MaxEntitiesOnWall];
    [SerializeField]
    private Transform[] entityPositions = new Transform[MaxEntitiesOnWall];
    [SerializeField]
    private GameObject dropPoint;
    [SerializeField]
    private bool addEnemies;

    public override void OnBuildingRemoved(ParentBuilding building) {
        base.OnBuildingRemoved(building);
        if (this.building == building) {
            RemoveAll();
        }
    }

    public bool DropEntity(bool byCommand) {
        bool canBeDropped = true;
        for (int i = 0; i < entitiesOnWall.Length; i++) {
            if (entitiesOnWall[i] != null) {
                entitiesOnWall[i].transform.position = dropPoint.transform.position;
                Entity entity = entitiesOnWall[i].GetComponent<Entity>();
                entity.Agent.enabled = true;
                entity.OnWall = false;
                entitiesOnWall[i] = null;
                return canBeDropped;
            }
        }
        if (byCommand) {
            EventLog.Instance.AddAction(LogType.Error, Messages.NoBowmanToDrop, transform.position);
        }
        return !canBeDropped;
    }

    public bool AddEntity(GameObject gameObjectToAdd) {
        bool canBeAdded = false;
        if (gameObjectToAdd == null) {
            EventLog.Instance.AddAction(LogType.Error, Messages.NoBowmanFoundNearWall, transform.position);
            return canBeAdded;
        }
        for (int i = 0; i < entitiesOnWall.Length; i++) {
            if (entitiesOnWall[i] == null) {
                entitiesOnWall[i] = gameObjectToAdd;
                Entity entity = entitiesOnWall[i].GetComponent<Entity>();
                entity.Stop();
                entity.Agent.enabled = false;
                entity.OnWall = true;
                entitiesOnWall[i].transform.position = entityPositions[i].position;
                if (!entitiesOnWall[i].activeSelf) {
                    entity.gameObject.SetActive(true);
                }
                canBeAdded = true;
                return canBeAdded;
            }
        }
        EventLog.Instance.AddAction(LogType.Error, Messages.YouCannotPlaceMoreBowmansOnWall, transform.position);
        return canBeAdded;
    }

    private void RemoveAll() {
        for (int i = 0; i < entitiesOnWall.Length; i++) {
            DropEntity(false);
        }
    }

    private void Start() {
        if (addEnemies) {
            for (int i = 0; i < entityPositions.Length; i++) {
                Entity entity = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Bowman).GetComponent<Entity>();
                entity.SetOwner(OwnerEnum.Enemy);
                AddEntity(entity.gameObject);
            }
        }
    }

}
