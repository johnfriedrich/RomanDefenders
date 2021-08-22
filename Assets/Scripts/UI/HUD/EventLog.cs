using System.Collections.Generic;
using Manager;
using Sound;
using UnityEngine;

namespace UI.HUD {
    public class EventLog : Menu {

        public static EventLog Instance { get; private set; }

        private List<EventLogItem> log = new List<EventLogItem>();
        [SerializeField]
        private int maxLogSize;
        [SerializeField]
        private GameObject scrollListParent;
        [SerializeField]
        private GameObject listPrefab;

        public void AddAction(LogType type, string action, Vector3 position) {
            if (!isActiveAndEnabled) {
                Show();
            }
            if (log.Count > maxLogSize) {
                EventLogItem temp = log[0];
                log.RemoveAt(0);
                Destroy(temp.gameObject);
            }
            EventLogItem eventLogItem = Instantiate(listPrefab, scrollListParent.transform).GetComponent<EventLogItem>();
            eventLogItem.Setup(position, action);
            log.Add(eventLogItem);
            SoundEnum clip;
            switch (type) {
                case LogType.Upgraded:
                    clip = SoundEnum.BuildingUpgradeComplete;
                    break;
                case LogType.Built:
                    clip = SoundEnum.BuildingBuilt;
                    break;
                case LogType.EnemyAttacksPlayer:
                    clip = SoundEnum.BuildingUpgradeComplete;
                    break;
                case LogType.EntityFinished:
                    clip = SoundEnum.EntitySwordHitMetal;
                    break;
                case LogType.Error:
                    clip = SoundEnum.EntityArrowImpact;
                    break;
                default:
                    clip = SoundEnum.UI_Button_Click;
                    break;
            }
            if (clip != SoundEnum.UI_Button_Click) {
                Sound.Sound.Instance.PlaySoundClip(clip);
            }
        }

        private void Awake() {
            Instance = this;
            Hide();
            EventManager.Instance.OnGameResetPostEvent += Clear;
        }

        private void Clear() {
            for (int i = 0; i < log.Count; i++) {
                if (log[i] != null) {
                    Destroy(log[i].gameObject);
                }
            }
            log.Clear();
        }

    }
}
