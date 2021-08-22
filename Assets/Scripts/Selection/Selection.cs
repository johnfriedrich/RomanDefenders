using System.Collections.Generic;
using System.Linq;
using Manager;
using Parent;
using UnityEngine;

namespace Selection {
    public class Selection : MonoBehaviour {

        private static Selection _instance;
        private List<ParentObject> selectedObjects = new List<ParentObject>();

        public static Selection Instance => _instance;

        public List<T> Get<T>() where T : ParentObject {
            return selectedObjects.OfType<T>().ToList();
        }

        public void Set(List<ParentObject> parentObjects) {

        }

        private void Awake() {
            _instance = this;
            EventManager.Instance.OnParentObjectSelectedEvent += SetSelectedObjects;
            EventManager.Instance.OnDeselectEvent += Clear;
        }

        private void SetSelectedObjects(List<ParentObject> parentObjects) {
            Clear();
            selectedObjects = parentObjects;
        }

        private void Clear() {
            selectedObjects.Clear();
        }

    }
}
