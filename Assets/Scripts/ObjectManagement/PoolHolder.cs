using System.Collections.Generic;
using System.Linq;
using Manager;
using Parent;
using UnityEngine;

namespace ObjectManagement {
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
        private readonly List<Pool> pools = new List<Pool>();

        public GameObject Entities => entities;

        public GameObject Buildings => buildings;

        public GameObject Arrows => arrows;

        public GameObject Particles => particles;

        public GameObject GetObjectActive(ParentObjectNameEnum type) {
            return GetRightPool(type).GetPoolObject(true);
        }

        public GameObject GetObject(ParentObjectNameEnum type) {
            return GetRightPool(type).GetPoolObject(false);
        }

        private Pool GetRightPool(ParentObjectNameEnum type) {
            if (pools.Count > 0)
            {
                foreach (var item in pools.Where(item => item.Type == type))
                {
                    return item;
                }
            }
            CreatePool(type);
            return GetRightPool(type);
        }

        private void CreatePool(ParentObjectNameEnum type) {
            var poolToAdd = gameObject.AddComponent<Pool>();
            poolToAdd.Type = type;
            poolToAdd.Size = 10;
            pools.Add(poolToAdd);
        }

        private void Awake() {
            Instance = this;
            EventManager.Instance.OnGameResetPreEvent += ResetObject;
        }

        private void ResetObject()
        {
            foreach (var t in pools)
            {
                t.Clear();
            }
        }

    }
}
