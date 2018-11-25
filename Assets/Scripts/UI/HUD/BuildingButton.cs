using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    private ParentObjectNameEnum buildingType;
    [SerializeField]
    private BuildingTooltip buildingTooltip;
    [SerializeField]
    private Image buttonImage;
    ParentBuilding building;

    public void GetBuilding() {
        if (!Manager.Instance.HasEnoughMana(PrefabHolder.Instance.GetInfo(buildingType).BuildCost)) {
            EventLog.Instance.AddAction(LogType.Error, "You don't have enough Mana!", Manager.Instance.EnemyTarget.transform.position);
            return;
        }
        if (Manager.Instance.BuildingPrefabOnMouse != null) {
            Manager.Instance.BuildingPrefabOnMouse.SetActive(false);
        }
        Manager.Instance.BuildingPrefabOnMouse = PoolHolder.Instance.GetObject(buildingType);
        Manager.Instance.BuildingPrefabOnMouse.AddComponent<BuildingPlacement>();
        Manager.Instance.BuildingPrefabOnMouse.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        buildingTooltip.SetTooltip(building.FriendlyName, building.Behaviour.Description, building.BuildCost, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData) {
        buildingTooltip.Hide();
    }

    private void Start() {
        building = (ParentBuilding)PrefabHolder.Instance.GetInfo(buildingType);
        buttonImage.sprite = building.IconSprite;
        buttonImage.preserveAspect = true;
    }

}
