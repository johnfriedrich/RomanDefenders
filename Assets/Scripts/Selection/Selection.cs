using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour {

    private static Selection instance;
    private List<ParentObject> selectedObjects = new List<ParentObject>();

    public static Selection Instance { get => instance; }

    public List<T> Get<T>() where T : ParentObject {
        List<T> temp = new List<T>();
        for (int i = 0; i < selectedObjects.Count; i++) {
            if (selectedObjects[i] is T) {
                temp.Add((T)selectedObjects[i]);
            }
        }
        return temp;
    }

    public void Set(List<ParentObject> parentObjects) {

    }

    private void Awake() {
        instance = this;
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
