using Entities;
using FoW;
using ObjectManagement;
using Parent;

public class Cheats : Menu {

	public void GetMeAnArmy() {
        for (int i = 0; i < 15; i++) {
            Entity entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Swordsman).GetComponent<Entity>();
            entityToTrain.gameObject.transform.position = Manager.Manager.Instance.EnemyTarget.transform.position;
            entityToTrain.SetOwner(OwnerEnum.Player);
            entityToTrain.gameObject.SetActive(true);
        }
        for (int i = 0; i < 3; i++) {
            Entity entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Catapult).GetComponent<Entity>();
            entityToTrain.gameObject.transform.position = Manager.Manager.Instance.EnemyTarget.transform.position;
            entityToTrain.SetOwner(OwnerEnum.Player);
            entityToTrain.gameObject.SetActive(true);
        }
        for (int i = 0; i < 15; i++) {
            Entity entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Bowman).GetComponent<Entity>();
            entityToTrain.gameObject.transform.position = Manager.Manager.Instance.EnemyTarget.transform.position;
            entityToTrain.SetOwner(OwnerEnum.Player);
            entityToTrain.gameObject.SetActive(true);
        }
        for (int i = 0; i < 15; i++) {
            Entity entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Horseman).GetComponent<Entity>();
            entityToTrain.gameObject.transform.position = Manager.Manager.Instance.EnemyTarget.transform.position;
            entityToTrain.SetOwner(OwnerEnum.Player);
            entityToTrain.gameObject.SetActive(true);
        }
    }

    public void ClearFog() {
        FogOfWar.instances[0].SetAll(0);
    }

    public void EnableFog() {
        FogOfWar.instances[0].SetAll(255);
    }

    public void AddMoveSpeed() {
        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (var item in entities) {
            if (item.IsPlayer()) {
                item.AddMovementSpeed(7);
            }
        }
    }

    public void RemoveSpeed() {
        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (var item in entities) {
            if (item.IsPlayer()) {
                item.ReduceMovementSpeed(7);
            }
        }
    }

}
