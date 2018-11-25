using UnityEngine;
using UnityEngine.EventSystems;

public class Deselector : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData) {
        if (eventData.button != PointerEventData.InputButton.Right) {
            EventManager.Instance.Deselect();
            Debug.Log("Deselected");
        }
    }

}
