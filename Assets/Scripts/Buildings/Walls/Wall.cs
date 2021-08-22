using Buildings.Behaviour;
using Entities;
using ObjectManagement;
using Parent;
using Translation;
using UI.HUD;
using UnityEngine;
using LogType = UI.HUD.LogType;

namespace Buildings.Walls {
    public class Wall : BuildingBehaviour {

        public static readonly int MaxEntitiesOnWall = 3;

        private readonly GameObject[] entitiesOnWall = new GameObject[MaxEntitiesOnWall];
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
            var canBeDropped = true;
            for (var i = 0; i < entitiesOnWall.Length; i++) {
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
            var canBeAdded = false;
            if (gameObjectToAdd == null) {
                EventLog.Instance.AddAction(LogType.Error, Messages.NoBowmanFoundNearWall, transform.position);
                return canBeAdded;
            }
            for (var i = 0; i < entitiesOnWall.Length; i++) {
                if (entitiesOnWall[i] == null) {
                    entitiesOnWall[i] = gameObjectToAdd;
                    var entity = entitiesOnWall[i].GetComponent<Entity>();
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
            for (var i = 0; i < entitiesOnWall.Length; i++) {
                DropEntity(false);
            }
        }

        private void Start() {
            if (!addEnemies) return;
            for (int i = 0; i < entityPositions.Length; i++) {
                Entity entity = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Bowman).GetComponent<Entity>();
                entity.SetOwner(OwnerEnum.Enemy);
                AddEntity(entity.gameObject);
            }
        }

    }
}
