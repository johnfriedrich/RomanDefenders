using System.Collections.Generic;
using System.IO;
using Entities;
using Manager;
using Parent;
using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class ParentObjectInfo : MonoBehaviour {

        [SerializeField]
        private Text entityInfoName;
        [SerializeField]
        private Image entityInfoLifeImage;
        [SerializeField]
        private Text entityInfoLife;
        [SerializeField]
        private Text entityInfoLevel;
        [SerializeField]
        private Text entityInfoDamage;
        [SerializeField]
        private Text entityInfoArmor;
        [SerializeField]
        private GameObject entityInfoDamageLabel;
        [SerializeField]
        private GameObject entityInfoArmorLabel;
        private ParentObject selectedObject;
        [SerializeField]
        private GameObject starRoot;
        [SerializeField]
        private GameObject starPrefab;
        private GameObject[] stars;

        private void SetStars(int amount) {
            if (stars == null) {
                GenerateStars();
            }
            starRoot.SetActive(true);
            foreach (var item in stars) {
                item.SetActive(false);
            }
            for (var i = 0; i < amount; i++) {
                stars[i].SetActive(true);
            }
        }

        private void GenerateStars() {
            stars = new GameObject[5];
            for (var i = 0; i < stars.Length; i++) {
                stars[i] = Instantiate(starPrefab, starRoot.transform);
            }
        }

        private void Start() {
            EventManager.Instance.OnParentObjectSelectedEvent += SetObject;
            EventManager.Instance.OnEntityDeathEvent += Disable;
            EventManager.Instance.OnBuildingRemovedEvent += Disable;
            EventManager.Instance.OnEntityDamageEvent += Refresh;
            EventManager.Instance.OnEntityHealEvent += Refresh;
            EventManager.Instance.OnEntityLevelUpEvent += Refresh;
            EventManager.Instance.OnBuildingDamageEvent += Refresh;
            EventManager.Instance.OnDeselectEvent += Deselect;
            gameObject.SetActive(false);
        }

        private void SetObject(List<ParentObject> selectedObjects) {
            gameObject.SetActive(true);
            selectedObject = selectedObjects[0];
            Show();
        }

        private void Deselect() {
            selectedObject = null;
            Clear();
            gameObject.SetActive(false);
        }

        private void Disable(ParentObject deadObject) {
            if (deadObject == selectedObject) {
                selectedObject = null;
                Clear();
                gameObject.SetActive(false);
            }
        }

        private void Disable(ParentObject parentObject, bool byEnemy) {
            Disable(parentObject);
        }

        private void Clear() {
            entityInfoName.text = string.Empty;
            entityInfoLife.text = string.Empty;
            entityInfoLifeImage.fillAmount = 100;
            entityInfoLevel.text = string.Empty;
            entityInfoDamage.text = string.Empty;
            entityInfoArmor.text = string.Empty;
            entityInfoDamageLabel.SetActive(false);
            entityInfoArmorLabel.SetActive(false);
            starRoot.SetActive(false);
            entityInfoLevel.gameObject.transform.parent.gameObject.SetActive(false);
        }

        private void Show() {
            Clear();
            if (selectedObject is Character) {
                var temp = (Character)selectedObject;
                entityInfoName.text = temp.CharacterName;
            } else {
                entityInfoName.text = selectedObject.FriendlyName;
            }
            entityInfoLifeImage.fillAmount = selectedObject.CurrentHealthPoints / selectedObject.MaxHealthpoints;
            entityInfoLife.text = selectedObject.CurrentHealthPoints.ToString() + Path.AltDirectorySeparatorChar + selectedObject.MaxHealthpoints;
            entityInfoLevel.text = selectedObject.CurrentLevel.ToString() + Path.AltDirectorySeparatorChar + selectedObject.MaxLevel;
            if (selectedObject is Entity o) {
                SetStars(o.CurrentLevel);
                entityInfoDamageLabel.SetActive(true);
                entityInfoArmorLabel.SetActive(true);
                entityInfoDamage.text = o.CalculateDamage().ToString();
                entityInfoArmor.text = o.GetArmor().ToString();
                return;
            }
            entityInfoLevel.gameObject.transform.parent.gameObject.SetActive(true);
        }

        private void Refresh(ParentObject damagedObject) {
            if (damagedObject == selectedObject) {
                Show();
            }
        }

    }
}
