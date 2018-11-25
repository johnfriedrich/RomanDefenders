using System.Collections.Generic;
using UnityEngine;

public class PoolHolder : MonoBehaviour {

    public static PoolHolder Instance { get; private set; }

    [SerializeField]
    private GameObject entities;
    [SerializeField]
    private GameObject buildings;
    [SerializeField]
    private GameObject arrows;
    [SerializeField]
    private GameObject particles;
    private List<Pool> pools = new List<Pool>();

    public GameObject Entities {
        get {
            return entities;
        }
    }

    public GameObject Buildings {
        get {
            return buildings;
        }
    }

    public GameObject Arrows {
        get {
            return arrows;
        }
    }

    public GameObject Particles {
        get {
            return particles;
        }
    }

    public GameObject GetObjectActive(ParentObjectNameEnum type) {
        return GetRightPool(type).GetPoolObject(true);
    }

    public GameObject GetObject(ParentObjectNameEnum type) {
        return GetRightPool(type).GetPoolObject(false);
    }

    private Pool GetRightPool(ParentObjectNameEnum type) {
        if (pools.Count > 0) {
            foreach (Pool item in pools) {
                if (item.Type == type) {
                    return item;
                }
            }
        }
        CreatePool(type);
        return GetRightPool(type);
    }

    private void CreatePool(ParentObjectNameEnum type) {
        Pool poolToAdd = gameObject.AddComponent<Pool>();
        poolToAdd.Type = type;
        poolToAdd.Size = 10;
        pools.Add(poolToAdd);
    }

    private void Awake() {
        Instance = this;
        EventManager.Instance.OnGameResetPreEvent += ResetObject;
    }

    private void ResetObject() {
        for (int i = 0; i < pools.Count; i++) {
            pools[i].Clear();
        }
    }

}
