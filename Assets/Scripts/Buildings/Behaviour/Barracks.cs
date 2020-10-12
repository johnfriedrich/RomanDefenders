using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : BuildingBehaviour {

    [SerializeField]
    private EntityQueue queue;
    [SerializeField]
    private GameObject movePoint;
    [SerializeField]
    private GameObject spawnPoint;
    private List<GameObject> entitiesToTrain = new List<GameObject>();
    [SerializeField]
    private NavMeshObstacle navMeshObstacle;
    [SerializeField]
    private NavMeshObstacle navMeshObstacleSide;
    private float trainFactor = 1;
    private bool alreadytraining;

    public Vector3 MovePoint {
        get {
            return movePoint.transform.localPosition;
        }

        set {
            movePoint.transform.localPosition = value;
        }
    }

    public bool Alreadytraining {
        get {
            return alreadytraining;
        }
    }

    public override void OnBuildingBuilt(ParentBuilding building) {
        base.OnBuildingBuilt(building);
        EventManager.Instance.OnEntityTrainFinishedEvent += TryTrainUnit;
        navMeshObstacle.enabled = true;
        navMeshObstacleSide.enabled = true;
    }

    public override void OnBuildingRemoved(ParentBuilding building) {
        base.OnBuildingRemoved(building);
        navMeshObstacle.enabled = false;
        navMeshObstacleSide.enabled = false;
    }

    public override bool LevelUp() {
        trainFactor -= 0.25f;
        return base.LevelUp();
    }

    public override bool Destroy() {
        if (alreadytraining) {
            EventLog.Instance.AddAction(LogType.Error, Messages.CantDestroyBuildingWhileTraining, transform.position);
            return false;
        }
        return base.Destroy();
    }

    public bool Train(TrainUnit unit) {
        bool canTrain = false;
        Entity temp = (Entity)PrefabHolder.Instance.GetInfo(unit.entityType);
        if (!Manager.Instance.HasEnoughMana(temp.BuildCost)) {
            EventLog.Instance.AddAction(LogType.Error, "You need " + temp.BuildCost + " " + Manager.Instance.CurrencyName + " to train " + temp.FriendlyName, transform.position);
            return canTrain;
        } else if (!Manager.Instance.HasEnoughFood(temp.FoodValue)) {
            EventLog.Instance.AddAction(LogType.Error, "You need " + temp.FoodValue + " " + Manager.Instance.FoodName + " to train " + temp.FriendlyName, transform.position);
            return canTrain;
        } else if (building.IsUpgrading) {
            EventLog.Instance.AddAction(LogType.Error, "You can't train while the building is upgrading", transform.position);
            return canTrain;
        }
        canTrain = true;
        Entity entityToTrain = PoolHolder.Instance.GetObject(unit.entityType).GetComponent<Entity>();
        entityToTrain.MarkedForTraining = true;
        queue.Put(entityToTrain);
        EventManager.Instance.EntityTrain(entityToTrain);
        if (!Alreadytraining) {
            entitiesToTrain.Add(entityToTrain.gameObject);
            TryTrainUnit();
            return canTrain;
        }
        entitiesToTrain.Add(entityToTrain.gameObject);
        return canTrain;
    }

    private void OnDisable() {
        EventManager.Instance.OnEntityTrainFinishedEvent -= TryTrainUnit;
        StopAllCoroutines();
        entitiesToTrain = new List<GameObject>();
        alreadytraining = false;
    }

    private void TryTrainUnit(Entity trainedEntity, BuildingBehaviour trainedIn) {
        if (trainedIn == this) {
            TryTrainUnit();
        }
    }

    private void TryTrainUnit() {
        if (entitiesToTrain.Count > 0) {
            StartCoroutine(TrainUnit());
        }
    }

    private IEnumerator TrainUnit() {
        alreadytraining = true;
        Entity spawnedEntity = entitiesToTrain[0].GetComponent<Entity>();
        yield return new WaitForSeconds(spawnedEntity.BuildTime * trainFactor);
        spawnedEntity.gameObject.transform.position = spawnPoint.transform.position;
        spawnedEntity.SetOwner(OwnerEnum.Player);
        spawnedEntity.gameObject.SetActive(true);
        spawnedEntity.MoveTo(movePoint.transform.position, false);
        spawnedEntity.MarkedForTraining = false;
        EventLog.Instance.AddAction(LogType.EntityFinished, "Trained " + spawnedEntity.FriendlyName, transform.position);
        entitiesToTrain.Remove(spawnedEntity.gameObject);
        alreadytraining = false;
        queue.Remove(spawnedEntity);
        EventManager.Instance.EntityTrainFinished(spawnedEntity, this);
    }

    private void OnEnable() {
        trainFactor = 1;
    }

}
