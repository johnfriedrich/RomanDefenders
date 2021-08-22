using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.HUD {
    public class EventLogItem: MonoBehaviour, IPointerDownHandler {

        private Vector3 position;
        [SerializeField]
        private TextMeshProUGUI text;

        public void OnPointerDown(PointerEventData eventData) {
            FlyCamera.Instance.JumpToTarget(position);
        }

        public void Setup(Vector3 position, string message) {
            this.position = position;
            text.text = message;
        }

    }
}
