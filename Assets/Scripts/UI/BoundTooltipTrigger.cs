using UnityEngine;
using UnityEngine.EventSystems;

public class BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    public string text;
    public Vector3 offset;

    public void OnPointerEnter(PointerEventData eventData) {
        StartHover(transform.position, offset);
    }

    public void OnSelect(BaseEventData eventData) {
        StartHover(transform.position, offset);
    }

    public void OnBecameInvisible() {
        StopHover();
    }

    public void OnPointerExit(PointerEventData eventData) {
        StopHover();
    }

    public void OnDeselect(BaseEventData eventData) {
        StopHover();
    }

    private void OnDisable() {
        StopHover();
    }

    private void StartHover(Vector3 currentPosition, Vector3 offset) {
        BoundTooltipItem.Instance.ShowTooltip(text, currentPosition, offset);
    }

    private void StopHover() {
        BoundTooltipItem.Instance.HideTooltip();
    }

}