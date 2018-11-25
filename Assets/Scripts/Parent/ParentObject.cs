using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public AudioSource AudioSource {
        get {
            return audioSource;
        }
    }

    public Renderer MeshRenderer {
        get {
            return mainRenderer;
        }
    }

    public int BuildCost {
        get {
            return buildCost;
        }
    }

    public int BuildTime {
        get {
            return buildTime;
        }
    }

    public float Armor {
        get {
            return armor;
        }
    }

    public InfoBar InfoBar {
        get {
            return infoBar;
        }
    }

    public GameObject CloseUpParent {
        get {
            return closeUpParent;
        }
    }

    public OwnerEnum Owner {
        get {
            return owner;
        }
    }

    public ParentObjectNameEnum ObjectName {
        get {
            return objectName;
        }
    }

    public int MaxLevel {
        get {
            return maxLevel;
        }
    }

    public float MaxHealthpoints {
        get {
            return maxHealthpoints;
        }
    }

    public float CurrentHealthPoints {
        get {
            return currentHealthPoints;
        }
    }

    public int CurrentLevel {
        get {
            return currentLevel;
        }
    }

    public string FriendlyName {
        get {
            if (friendlyName == string.Empty) {
                return objectName.ToString();
            } else {
                return friendlyName;
            };
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
        if (IsAlive() && IsPlayer() && isActiveAndEnabled && Manager.Instance.BuildingPrefabOnMouse == null && !Manager.Instance.SkillOnMouse) {
            List<ParentObject> temp = new List<ParentObject> {
                this
            };
            EventManager.Instance.ParentObjectSelected(temp);
            StartCoroutine(HackSelector());
        }
    }

    protected virtual void OnEnable() {
        HideCircle();
        audioSource.volume = Sound.Instance.SoundVolume;
    }

    protected virtual void Reset() {
        //foreach (Renderer rend in gameObject.GetComponentsInChildren<Renderer>()) {
        //    rend.enabled = true;
        //}
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
