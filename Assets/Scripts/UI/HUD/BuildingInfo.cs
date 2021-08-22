using System.Collections.Generic;
using Manager;
using Parent;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class BuildingInfo : MonoBehaviour {

        [SerializeField]
        private Text description;
        private ParentBuilding selectedBuilding;

        void Start() {
            EventManager.Instance.OnParentObjectSelectedEvent += SetBuilding;
            EventManager.Instance.OnBuildingRemovedEvent += Deselect;
            EventManager.Instance.OnDeselectEvent += Deselect;
            EventManager.Instance.OnDeselectEvent += Deselect;
            gameObject.SetActive(false);
        }

        private void SetBuilding(List<ParentObject> selectedObjects) {
            Deselect();
            if (selectedObjects[0] is ParentBuilding) {
                if (selectedObjects[0] != selectedBuilding) {
                    selectedBuilding = (ParentBuilding)selectedObjects[0];
                }
                Show();
                return;
            }
        }

        private void Deselect(ParentObject parentObject, bool byEnemy) {
            Deselect();
        }

        private void Deselect() {
            Clear();
            Hide();
        }

        private void Show() {
            Clear();
            description.text = selectedBuilding.Behaviour.Description;
            gameObject.SetActive(true);
        }

        private void Hide() {
            gameObject.SetActive(false);
        }

        private void Clear() {
            description.text = string.Empty;
        }

    }
}
