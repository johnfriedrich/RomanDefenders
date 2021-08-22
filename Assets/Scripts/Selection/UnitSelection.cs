using System.Collections.Generic;
using Entities;
using Manager;
using Parent;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Selection {
    public class UnitSelection : MonoBehaviour {

        private bool isSelecting;
        private Vector3 mousePosition1;
        [SerializeField]
        private new Camera camera;

        private void Start() {
            EventManager.Instance.OnStartGamePostEvent += Enable;
            EventManager.Instance.OnQuitGameEvent += Disable;
            enabled = false;
        }

        private void Enable() {
            enabled = true;
        }

        private void Disable() {
            enabled = false;
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject == null) {
                isSelecting = true;
                mousePosition1 = Input.mousePosition;
            }

            if (isSelecting) {
                foreach (Entity entity in FindObjectsOfType<Entity>()) {
                    if (CanBeSelected(entity)) {
                        if (IsWithinSelectionBounds(entity.gameObject)) {
                            entity.ShowCircle();
                            entity.InfoBar.Show();
                        } else {
                            entity.HideCircle();
                            entity.InfoBar.Hide();
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                List<ParentObject> temp = new List<ParentObject>();
                foreach (Entity entity in FindObjectsOfType<Entity>()) {
                    if (IsWithinSelectionBounds(entity.gameObject) && CanBeSelected(entity)) {
                        temp.Add(entity);
                        entity.InfoBar.Show();
                    }
                    if (temp.Count >= 25) {
                        break;
                    }

                }

                if (temp.Count > 0) {
                    EventManager.Instance.Deselect();
                    EventManager.Instance.ParentObjectSelected(temp);
                    foreach (ParentObject item in temp) {
                        item.InfoBar.Show();
                        item.InfoBar.UpdateBar();
                    }
                }
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                isSelecting = false;
            }

        }

        private bool CanBeSelected(Entity entity) {
            return entity != null && entity.IsAlive() && entity.IsPlayer() && entity != entity.OnWall;
        }

        private bool IsWithinSelectionBounds(GameObject gameObject) {
            if (!isSelecting)
                return false;
            Bounds viewportBounds = Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }

        private void OnGUI() {
            if (isSelecting) {
                Rect rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f ));
                Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f ));
            }
        }

    }
}