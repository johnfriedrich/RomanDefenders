using System.IO;
using Entities;
using Parent;
using UnityEngine;
using UnityEngine.UI;

public class InfoBar : Menu {

    [SerializeField]
    private Image healthImage;
    [SerializeField]
    private Text entityName;
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private GameObject starRoot;
    [SerializeField]
    private GameObject starPrefab;
    private GameObject[] stars;
    private ParentObject entity;
    private new Camera camera;

    private void GenerateStars() {
        stars = new GameObject[5];
        for (int i = 0; i < stars.Length; i++) {
            stars[i] = Instantiate(starPrefab, starRoot.transform);
        }
    }

    public void UpdateBar() {
        if (entity == null) {
            entity = GetComponentInParent<ParentObject>();
        }
        healthImage.fillAmount = entity.CurrentHealthPoints / entity.MaxHealthpoints;
        healthText.text = entity.CurrentHealthPoints.ToString() + Path.AltDirectorySeparatorChar + entity.MaxHealthpoints;
        if (entity is Entity) {
            SetStars(entity.CurrentLevel);
        }
    }

    public override void Hide() {
        if (entity == null) {
            entity = GetComponentInParent<ParentObject>();
        }
        if (entity.CurrentHealthPoints == entity.MaxHealthpoints || !entity.enabled) {
            base.Hide();
        }
    }

    public bool IsEnabled() {
        return gameObject.activeSelf;
    }

    private void OnEnable() {
        camera = Camera.main;
        GetComponent<Canvas>().worldCamera = camera;
        entity = GetComponentInParent<ParentObject>();
        if (entity is Entity) {
            SetStars(entity.CurrentLevel);
        }
        if (entity is Character) {
            Character temp = (Character)entity;
            entityName.text = temp.CharacterName;
        } else {
            entityName.text = entity.FriendlyName;
        }
    }

    private void Update() {
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, camera.transform.rotation * Vector3.up);
    }

    private void SetStars(int amount) {
        if (stars == null) {
            GenerateStars();
        }
        foreach (var item in stars) {
            item.SetActive(false);
        }
        for (int i = 0; i < amount; i++) {
            stars[i].SetActive(true);
        }
    }

}
