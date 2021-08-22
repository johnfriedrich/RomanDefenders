using System.Collections.Generic;
using Entities;
using Manager;
using UnityEngine;

namespace Buildings.Behaviour {
    public class EntityQueue : MonoBehaviour {

        private List<QueueItem> queue = new List<QueueItem>();
        [SerializeField]
        private QueueItem queueItem;

        public void Put(Entity entity) {
            QueueItem item = null;
            foreach (var t in queue) {
                if (t.Type == entity.ObjectName) {
                    item = t;
                    break;
                }
            }
            if (item != null) {
                item.Add();
            } else {
                item = Instantiate(queueItem, transform);
                item.Type = entity.ObjectName;
                item.Add();
                queue.Add(item);
            }
        }

        public void Remove(Entity entity) {
            QueueItem item = null;
            for (int i = 0; i < queue.Count; i++) {
                if (queue[i].Type == entity.ObjectName) {
                    item = queue[i];
                    break;
                }
            }
            if (item != null) {
                item.Remove();
            }
        }

        private void Clear() {
            foreach (var t in queue) {
                Destroy(t.gameObject);
            }

            queue = new List<QueueItem>();
        }

        private void Start() {
            EventManager.Instance.OnGameResetEvent += Clear;
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

    }
}
