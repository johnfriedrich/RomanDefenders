
using UnityEngine;

public class Settler : EntityBehaviour {

    private ParentBuilding currentBuilding;

    public ParentBuilding CurrentBuilding { set => currentBuilding = value; }

    public override void StartMove() {
        if (currentBuilding != null) {
            currentBuilding.DispenseSettler(this);
        }
        base.StartMove();
    }

    public override void EndMove() {
        base.EndMove();
        if (currentBuilding != null) {
            //anim hammer hit
        }
    }

}
