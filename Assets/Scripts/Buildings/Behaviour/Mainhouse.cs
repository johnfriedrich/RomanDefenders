using System.Collections;
using System.Collections.Generic;
using Entities;
using Manager;
using ObjectManagement;
using Parent;
using UI.CharacterEditor;
using UI.HUD;
using UnityEngine;
using LogType = UI.HUD.LogType;

namespace Buildings.Behaviour {
    public class Mainhouse : BuildingBehaviour {

        [SerializeField]
        private GameObject spawnPoint;
        [SerializeField]
        private EntityQueue queue;
        private List<GameObject> entitiesToTrain = new List<GameObject>();
        private float costFactor = 1;
        private bool alreadytraining;

        public override void OnBuildingBuilt(ParentBuilding building) {
            base.OnBuildingBuilt(building);
            if (this.building.IsPlayer()) {
                SpawnDefaults();
                EventManager.Instance.OnEntityTrainFinishedEvent += TryTrainUnit;
            }
        }

        public bool Alreadytraining => alreadytraining;

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

        public bool Train(TrainUnit unit) {
            var canTrain = false;
            var temp = (Entity)PrefabHolder.Instance.GetInfo(unit.entityType);
            if (!Manager.Manager.Instance.HasEnoughMana(temp.BuildCost)) {
                EventLog.Instance.AddAction(LogType.Error, "You need " + temp.BuildCost + " " + Manager.Manager.Instance.CurrencyName + " to train " + temp.FriendlyName, transform.position);
                return canTrain;
            }

            if (!Manager.Manager.Instance.HasEnoughFood(temp.FoodValue)) {
                EventLog.Instance.AddAction(LogType.Error, "You need " + temp.FoodValue + " " + Manager.Manager.Instance.FoodName + " to train " + temp.FriendlyName, transform.position);
                return canTrain;
            }

            if (building.IsUpgrading) {
                EventLog.Instance.AddAction(LogType.Error, "You can't train while the building is upgrading", transform.position);
                return canTrain;
            }
            canTrain = true;
            var entityToTrain = PoolHolder.Instance.GetObject(unit.entityType).GetComponent<Entity>();
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
            var spawnedEntity = entitiesToTrain[0].GetComponent<Entity>();
            yield return new WaitForSeconds(spawnedEntity.BuildTime * costFactor);
            spawnedEntity.gameObject.transform.position = spawnPoint.transform.position;
            spawnedEntity.SetOwner(OwnerEnum.Player);
            spawnedEntity.gameObject.SetActive(true);
            spawnedEntity.MoveTo(spawnPoint.transform.position, false);
            spawnedEntity.MarkedForTraining = false;
            EventLog.Instance.AddAction(LogType.EntityFinished, "Trained " + spawnedEntity.FriendlyName, transform.position);
            entitiesToTrain.Remove(spawnedEntity.gameObject);
            alreadytraining = false;
            queue.Remove(spawnedEntity);
            EventManager.Instance.EntityTrainFinished(spawnedEntity, this);
        }

        private bool SpawnHero(bool firstSpawn) {
            bool canBeSpawned = false;
            Character hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>();
            hero.Load(CharacterEditor.Instance.CharacterSaveData);
            if (!firstSpawn) {
                if (!Manager.Manager.Instance.HasEnoughMana(hero.RevivalCost * costFactor)) {
                    EventLog.Instance.AddAction(LogType.Error, "You need more Mana!", transform.position);
                    return canBeSpawned;
                }

                if (hero.IsAlive()) {
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
            if (!building.IsPlayer()) return;
            Entity entityToTrain;
            for (var i = 0; i < 3; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Swordsman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);
            }
            for (var i = 0; i < 3; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Bowman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);

            }
            for (var i = 0; i < 1; i++) {
                entityToTrain = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Horseman).GetComponent<Entity>();
                SpawnHelper(entityToTrain);

            }
            SpawnHero(true);
        }

        private void SpawnHelper(Entity entity) {
            entity.gameObject.transform.position = spawnPoint.transform.position;
            entity.SetOwner(OwnerEnum.Player);
            entity.gameObject.SetActive(true);
        }

        private void OnDisable() {
            EventManager.Instance.OnEntityTrainFinishedEvent -= TryTrainUnit;
            StopAllCoroutines();
            entitiesToTrain = new List<GameObject>();
            alreadytraining = false;
        }

        private void OnEnable() {
            costFactor = 1;
        }

    }
}
