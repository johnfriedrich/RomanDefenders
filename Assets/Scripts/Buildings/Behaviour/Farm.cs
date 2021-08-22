using UnityEngine;

namespace Buildings.Behaviour {
    public class Farm : BuildingBehaviour {

        private int foodValue = 10;
        [SerializeField]
        private int increasePerLevel;

        public override void OnBuildingBuilt(ParentBuilding building) {
            base.OnBuildingBuilt(building);
            if (building.IsPlayer()) {
                Manager.Manager.Instance.AddFood(foodValue);
            }
        }

        public override bool LevelUp() {
            foodValue += increasePerLevel;
            Manager.Manager.Instance.AddFood(increasePerLevel);
            return base.LevelUp();
        }

        public override void OnBuildingRemoved(ParentBuilding building) {
            base.OnBuildingRemoved(building);
            if (building.IsPlayer()) {
                Manager.Manager.Instance.RemoveFood(foodValue);
            }
        }

        private void OnEnable() {
            foodValue = 10;
        }

    }
}
