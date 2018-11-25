using UnityEngine;

public class BuildingBehaviour: MonoBehaviour {

    [SerializeField]
    private string description;
    protected ParentBuilding building;

    public string Description {
        get {
            return description;
        }
    }

    public virtual void OnBuildingBuilt(ParentBuilding building) {
        this.building = building;
        if (building.IsPlayer() && building.ObjectName != ParentObjectNameEnum.Mainhouse) {
            Sound.Instance.PlaySoundClipWithSource(new SoundHolder(SoundEnum.BuildingBuilt, 1, 0), building.AudioSource, 0);
            EventLog.Instance.AddAction(LogType.Built, building.FriendlyName + " finished", transform.position);
        }
    }

    public virtual void OnBuildingRemoved(ParentBuilding building) {}

    public virtual bool LevelUp() { return true; }

    public virtual bool Destroy() { return true; }

}
