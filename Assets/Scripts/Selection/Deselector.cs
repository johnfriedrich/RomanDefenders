using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Selection {
    public class Deselector : MonoBehaviour, IPointerDownHandler {

        public void OnPointerDown(PointerEventData eventData) {
            if (eventData.button != PointerEventData.InputButton.Right) {
                EventManager.Instance.Deselect();
                Debug.Log("Deselected");
            }
        }

    }
}
