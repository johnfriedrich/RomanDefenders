using System.Collections;
using System.Collections.Generic;
using Entities;
using Interfaces;
using Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Parent {
    public class ParentObject : MonoBehaviour, IDamageable, IRepairable, IPointerDownHandler {

        [SerializeField]
        protected GameObject selectionCircle;
        [SerializeField]
        protected string friendlyName;
        [SerializeField]
        protected OwnerEnum owner;
        [SerializeField]
        protected ParentObjectNameEnum objectName;
        [SerializeField]
        protected int maxLevel;
        [SerializeField]
        protected int currentLevel;
        [SerializeField]
        protected float maxHealthpoints;
        [SerializeField]
        protected float currentHealthPoints;
        [SerializeField]
        protected Renderer mainRenderer;
        [SerializeField]
        protected int buildCost;
        [SerializeField]
        protected int buildTime;
        [SerializeField]
        protected InfoBar infoBar;
        [SerializeField]
        protected float armor;
        protected float originalArmor;
        [SerializeField]
        protected AudioSource audioSource;
        protected EntityStateEnum entityState;

        [SerializeField]
        private GameObject closeUpParent;
        private BoxCollider boxCollider;

        public AudioSource AudioSource => audioSource;

        public Renderer MeshRenderer => mainRenderer;

        public int BuildCost => buildCost;

        public int BuildTime => buildTime;

        public float Armor => armor;

        public InfoBar InfoBar => infoBar;

        public GameObject CloseUpParent => closeUpParent;

        public OwnerEnum Owner => owner;

        public ParentObjectNameEnum ObjectName => objectName;

        public int MaxLevel => maxLevel;

        public float MaxHealthpoints => maxHealthpoints;

        public float CurrentHealthPoints => currentHealthPoints;

        public int CurrentLevel {
            get {
                return currentLevel;
            }
        }

        public string FriendlyName {
            get {
                if (friendlyName == string.Empty) {
                    return objectName.ToString();
                }

                return friendlyName;
            }
            set {
                friendlyName = value;
            }
        }

        public Vector3 GetHitpoint(Vector3 position) {
            if (boxCollider == null) {
                boxCollider = GetComponent<BoxCollider>();
            }
            return boxCollider.ClosestPoint(position);
        }

        public virtual bool LevelUp() {
            return true;
        }

        public bool IsPlayer() {
            return owner == OwnerEnum.Player;
        }

        public bool IsMaxLevel() {
            return currentLevel == maxLevel;
        }

        public bool IsAlive() => entityState == EntityStateEnum.Alive;

        public void ShowCircle() {
            if (selectionCircle != null) {
                selectionCircle.SetActive(true);
            }
        }

        public void HideCircle() {
            if (selectionCircle != null) {
                selectionCircle.SetActive(false);
            }
        }

        public bool IsSelected() {
            return selectionCircle.activeSelf;
        }

        public virtual void TakeDamage(float damage) { }

        public float CalculateDamage() {
            return 0;
        }

        public void RepairSelf(int amount) { }

        public virtual void OnPointerDown(PointerEventData eventData) {
            if (IsAlive() && IsPlayer() && isActiveAndEnabled && Manager.Manager.Instance.BuildingPrefabOnMouse == null && !Manager.Manager.Instance.SkillOnMouse) {
                var temp = new List<ParentObject> {
                    this
                };
                EventManager.Instance.ParentObjectSelected(temp);
                StartCoroutine(HackSelector());
            }
        }

        protected virtual void OnEnable() {
            HideCircle();
            audioSource.volume = Sound.Sound.Instance.SoundVolume;
        }

        protected virtual void Reset() {
            currentHealthPoints = maxHealthpoints;
            entityState = EntityStateEnum.Alive;
            currentLevel = 1;
            infoBar.Show();
            infoBar.UpdateBar();
            infoBar.Hide();
        }

        protected virtual void BackToPool() {
            gameObject.SetActive(false);
        }

        //Last Minute Fix
        private IEnumerator HackSelector() {
            yield return new WaitForSeconds(0.2f);
            infoBar.Show();
            infoBar.UpdateBar();
            ShowCircle();
        }

    }
}
