using Parent;
using UnityEngine;

namespace ObjectManagement {
    public class PrefabHolder : MonoBehaviour {

        public static PrefabHolder Instance { get; private set; }

        [SerializeField]
        private GameObject characterPrefab;
        private GameObject characterPrefabInternal;
        [SerializeField]
        private GameObject swordsmanPrefab;
        [SerializeField]
        private GameObject bowmanPrefab;
        [SerializeField]
        private GameObject horsemanPrefab;
        [SerializeField]
        private GameObject catapultPrefab;
        [SerializeField]
        private GameObject forgePrefab;
        [SerializeField]
        private GameObject mainhousePrefab;
        [SerializeField]
        private GameObject farmPrefab;
        [SerializeField]
        private GameObject minePrefab;
        [SerializeField]
        private GameObject barracksPrefab;
        [SerializeField]
        private GameObject woodenWallPrefab;
        [SerializeField]
        private GameObject stoneWallPrefab;
        [SerializeField]
        private GameObject stoneTowerPrefab;
        [SerializeField]
        private GameObject arrowPrefab;
        [SerializeField]
        private GameObject stonePrefab;
        [SerializeField]
        private GameObject upgradeParticlesPrefab;
        [SerializeField]
        private GameObject fireballPrefab;
        [SerializeField]
        private GameObject deathAngelPrefab;
        [SerializeField]
        private GameObject deathObjectPrefab;
        [SerializeField]
        private GameObject fireballImpactPrefab;
        [SerializeField]
        private GameObject iceballImpactPrefab;
        [SerializeField]
        private GameObject healImpactPrefab;
        [SerializeField]
        private GameObject selectionCirclePrefab;
        [SerializeField]
        private GameObject settlerPrefab;

        //Internal copies
        private GameObject swordsmanPrefabInternal;
        private GameObject bowmanPrefabInternal;
        private GameObject horsemanPrefabInternal;
        private GameObject catapultPrefabInternal;
        private GameObject forgePrefabInternal;
        private GameObject mainhousePrefabInternal;
        private GameObject farmPrefabInternal;
        private GameObject minePrefabInternal;
        private GameObject barracksPrefabInternal;
        private GameObject woodenWallPrefabInternal;
        private GameObject stoneWallPrefabInternal;
        private GameObject stoneTowerPrefabInternal;
        private GameObject arrowPrefabInternal;
        private GameObject stonePrefabInternal;
        private GameObject upgradeParticlesPrefabInternal;
        private GameObject fireballPrefabInternal;
        private GameObject deathAngelPrefabInternal;
        private GameObject deathObjectPrefabInternal;
        private GameObject fireballImpactPrefabInternal;
        private GameObject iceballImpactPrefabInternal;
        private GameObject healImpactPrefabInternal;
        private GameObject selectionCirclePrefabInternal;
        private GameObject settlerPrefabInternal;

        public ParentObject GetInfo(ParentObjectNameEnum objectNameEnum) {
            return Get(objectNameEnum).GetComponent<ParentObject>();
        }

        public GameObject Get(ParentObjectNameEnum objectNameEnum) {
            switch (objectNameEnum) {
                case ParentObjectNameEnum.Character:
                    return characterPrefabInternal;
                case ParentObjectNameEnum.Swordsman:
                    return swordsmanPrefabInternal;
                case ParentObjectNameEnum.Bowman:
                    return bowmanPrefabInternal;
                case ParentObjectNameEnum.Horseman:
                    return horsemanPrefabInternal;
                case ParentObjectNameEnum.Catapult:
                    return catapultPrefabInternal;
                case ParentObjectNameEnum.Forge:
                    return forgePrefabInternal;
                case ParentObjectNameEnum.Mainhouse:
                    return mainhousePrefabInternal;
                case ParentObjectNameEnum.Farm:
                    return farmPrefabInternal;
                case ParentObjectNameEnum.Mine:
                    return minePrefabInternal;
                case ParentObjectNameEnum.Barracks:
                    return barracksPrefabInternal;
                case ParentObjectNameEnum.WoodWall:
                    return woodenWallPrefabInternal;
                case ParentObjectNameEnum.StoneWall:
                    return stoneWallPrefabInternal;
                case ParentObjectNameEnum.StoneTower:
                    return stoneTowerPrefabInternal;
                case ParentObjectNameEnum.Arrow:
                    return arrowPrefabInternal;
                case ParentObjectNameEnum.Stone:
                    return stonePrefabInternal;
                case ParentObjectNameEnum.UpgradeParticles:
                    return upgradeParticlesPrefabInternal;
                case ParentObjectNameEnum.DeathAngel:
                    return deathAngelPrefabInternal;
                case ParentObjectNameEnum.DeathObject:
                    return deathObjectPrefabInternal;
                case ParentObjectNameEnum.FireballImpact:
                    return fireballImpactPrefabInternal;
                case ParentObjectNameEnum.IceballImpact:
                    return iceballImpactPrefabInternal;
                case ParentObjectNameEnum.HealImpact:
                    return healImpactPrefabInternal;
                case ParentObjectNameEnum.SelectionCircle:
                    return selectionCirclePrefabInternal;
                case ParentObjectNameEnum.Settler:
                    return settlerPrefabInternal;
                default:
                    return null;
            }
        }

        private void Awake() {
            Instance = this;
            LoadClones();
        }

        private void LoadClones() {
            swordsmanPrefabInternal = Instantiate(swordsmanPrefab, transform);
            bowmanPrefabInternal = Instantiate(bowmanPrefab, transform);
            horsemanPrefabInternal = Instantiate(horsemanPrefab, transform);
            catapultPrefabInternal = Instantiate(catapultPrefab, transform);
            forgePrefabInternal = Instantiate(forgePrefab, transform);
            mainhousePrefabInternal = Instantiate(mainhousePrefab, transform);
            farmPrefabInternal = Instantiate(farmPrefab, transform);
            minePrefabInternal = Instantiate(minePrefab, transform);
            barracksPrefabInternal = Instantiate(barracksPrefab, transform);
            woodenWallPrefabInternal = Instantiate(woodenWallPrefab, transform);
            stoneWallPrefabInternal = Instantiate(stoneWallPrefab, transform);
            stoneTowerPrefabInternal = Instantiate(stoneTowerPrefab, transform);
            arrowPrefabInternal = Instantiate(arrowPrefab, transform);
            stonePrefabInternal = Instantiate(stonePrefab, transform);
            upgradeParticlesPrefabInternal = Instantiate(upgradeParticlesPrefab, transform);
            deathAngelPrefabInternal = Instantiate(deathAngelPrefab, transform);
            deathObjectPrefabInternal = Instantiate(deathObjectPrefab, transform);
            fireballImpactPrefabInternal = Instantiate(fireballImpactPrefab, transform);
            iceballImpactPrefabInternal = Instantiate(iceballImpactPrefab, transform);
            healImpactPrefabInternal = Instantiate(healImpactPrefab, transform);
            characterPrefabInternal = Instantiate(characterPrefab, transform);
            selectionCirclePrefabInternal = Instantiate(selectionCirclePrefab, transform);
            settlerPrefabInternal = Instantiate(settlerPrefab, transform);
        }

    }
}
