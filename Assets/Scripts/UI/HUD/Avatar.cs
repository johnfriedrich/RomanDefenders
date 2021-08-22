using System.Collections.Generic;
using Manager;
using Parent;
using UnityEngine;

namespace UI.HUD {
    public class Avatar : Menu {

        [SerializeField]
        private GameObject closeUp;
        private ParentObject current;

        private void Start() {
            EventManager.Instance.OnParentObjectSelectedEvent += AddCloseUp;
            EventManager.Instance.OnEntityDeathEvent += Disable;
            EventManager.Instance.OnBuildingRemovedEvent += Disable;
            EventManager.Instance.OnDeselectEvent += Hide;
            Hide();
        }

        private void AddCloseUp(List<ParentObject> parentObjects) {
            Show();
            current = parentObjects[0];
            var parentFromRender = RenderView.Instance.GetRenderObject(current.ObjectName).GetComponent<ParentObject>();
            closeUp.transform.SetParent(parentFromRender.CloseUpParent.transform, false);
            closeUp.transform.position = parentFromRender.CloseUpParent.transform.position;
        }

        private void Disable(ParentObject parentObject) {
            if (current == parentObject) {
                Hide();
            }
        }

        private void Disable(ParentObject parentObject, bool byEnemy) {
            Disable(parentObject);
        }

    }
}
