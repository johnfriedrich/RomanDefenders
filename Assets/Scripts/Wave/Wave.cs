using System;
using System.Collections;
using System.Collections.Generic;
using Entities;
using Manager;
using ObjectManagement;
using Parent;
using UnityEngine;

namespace Wave {
    public class Wave : MonoBehaviour {

        [SerializeField]
        private WaveObject[] enemies;
        [SerializeField]
        private float warmupTime;
        private float remainingWarmupTime;
        [SerializeField]
        private float timeBetweenSpawns;
        private readonly List<Entity> tempEntities = new List<Entity>();
        private int currentEnemy = 0;
        private int enemiesAlive;

        public int RemainingWarmupTime {
            get {
                if (remainingWarmupTime > 0) {
                    return Convert.ToInt32(remainingWarmupTime);
                }

                return 0;
            }
        }

        public float WarmupTime => warmupTime;

        public void StartWave() {
            StartCoroutine(Spawner());
        }

        private void Start() {
            EventManager.Instance.OnEntityDeathEvent += DecreaseEnemyCount;
            remainingWarmupTime = warmupTime;
        }

        private void Update() {
            if (remainingWarmupTime > 0) {
                remainingWarmupTime -= Time.deltaTime;
            }
        }

        private IEnumerator Spawner() {
            yield return new WaitForSecondsRealtime(timeBetweenSpawns);
            SpawnNext();
        }

        private void DecreaseEnemyCount(Entity entity) {
            if (!tempEntities.Contains(entity)) return;
            enemiesAlive--;
            tempEntities.Remove(entity);
            if (enemiesAlive == 0) {
                EventManager.Instance.WaveFinish();
            }
        }

        private void SpawnNext() {
            if (Manager.Manager.Instance.IsPaused) {
                StartCoroutine(Spawner());
                Debug.Log("Waiting");
                return;
            }
            var enemy = PoolHolder.Instance.GetObject(enemies[currentEnemy].EntityName).GetComponent<Entity>();
            enemy.gameObject.transform.position = Manager.Manager.Instance.EnemySpawn.transform.position;
            enemy.SetOwner(OwnerEnum.Enemy);
            enemy.gameObject.SetActive(true);
            tempEntities.Add(enemy);
            for (int i = 1; i < enemies[currentEnemy].EntityLevel; i++) {
                enemy.LevelUp();
            }
            enemy.MoveTo(Manager.Manager.Instance.EnemyTarget.transform.position, true);
            currentEnemy++;
            enemiesAlive++;
            if (currentEnemy == enemies.Length) {
                return;
            }
            StartCoroutine(Spawner());
        }

    }
}
