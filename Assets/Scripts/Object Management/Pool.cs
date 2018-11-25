using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {

    public ParentObjectNameEnum Type;
    public int Size;

    private List<GameObject> pool = new List<GameObject>();

    public void Clear() {
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].GetComponent<Entity>() != null) {
                pool[i].GetComponent<Entity>().Stop();
            }
            pool[i].SetActive(false);
        }
    }

    public GameObject GetPoolObject(bool active) {
        if (pool.Count == 0) {
            PopulatePool();
        }
        foreach (GameObject item in pool) {
            if (active) {
                if (item.activeInHierarchy) {
                    return item;
                }
            }
            if (!item.activeInHierarchy && (Type == ParentObjectNameEnum.Arrow || Type == ParentObjectNameEnum.UpgradeParticles || Type == ParentObjectNameEnum.Stone || Type == ParentObjectNameEnum.DeathAngel)) {
                return item;
            }
            if (!item.activeInHierarchy && item.GetComponent<ParentObject>() is ParentBuilding) {
                return item;
            }
            if (!item.activeInHierarchy && item.GetComponent<ParentObject>() is Entity && !item.GetComponent<Entity>().MarkedForTraining) {
                return item;
            }
        }
        return CreateEntity(Type);
    }

    private GameObject CreateEntity(ParentObjectNameEnum type) {
        Transform parent;
        if (type == ParentObjectNameEnum.Swordsman || type == ParentObjectNameEnum.Horseman || type == ParentObjectNameEnum.Bowman || type == ParentObjectNameEnum.DeathAngel || type == ParentObjectNameEnum.Character || type == ParentObjectNameEnum.Catapult) {
            parent = PoolHolder.Instance.Entities.transform;
        } else if (type == ParentObjectNameEnum.Arrow || type == ParentObjectNameEnum.Stone) {
            parent = PoolHolder.Instance.Arrows.transform;
        } else if (type == ParentObjectNameEnum.UpgradeParticles || type == ParentObjectNameEnum.FireballImpact || type == ParentObjectNameEnum.IceballImpact || type == ParentObjectNameEnum.HealImpact) {
            parent = PoolHolder.Instance.Particles.transform;
        } else {
            parent = PoolHolder.Instance.Buildings.transform;
        }
        GameObject gObject = Instantiate(PrefabHolder.Instance.Get(type), parent);
        pool.Add(gObject);
        return gObject;
    }

    private void PopulatePool() {
        if (Type == ParentObjectNameEnum.Character) {
            Size = 1;
        }
        for (int i = 0; i < Size; i++) {
            CreateEntity(Type);
        }
    }

}
