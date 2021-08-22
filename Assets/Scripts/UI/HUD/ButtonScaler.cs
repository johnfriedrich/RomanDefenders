using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.HUD {
    public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

        [SerializeField]
        private GameObject button;
        private RectTransform rectTransform;

        public void OnPointerEnter(PointerEventData eventData) {
            rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        }

        public void OnPointerExit(PointerEventData eventData) {
            rectTransform.localScale = new Vector3(1, 1, 1);
        }

        private void Start() {
            rectTransform = button.GetComponent<RectTransform>();
        }

    }
}
