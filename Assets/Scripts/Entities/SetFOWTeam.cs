using FoW;
using Parent;
using UnityEngine;

namespace Entities {
    public class SetFOWTeam : MonoBehaviour {

        private ParentObject parent;
        private FogOfWarUnit fogOfWarUnit;

        private void Awake() {
            parent = GetComponent<ParentObject>();
            fogOfWarUnit = GetComponent<FogOfWarUnit>();
        }

        private void OnEnable() {
            fogOfWarUnit.team = (int)parent.Owner;
        }
    }
}
