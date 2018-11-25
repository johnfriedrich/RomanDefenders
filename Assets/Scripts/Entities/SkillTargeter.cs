using UnityEngine;

public class SkillTargeter : MonoBehaviour {

    private const string Distance = "Too far away";

    private bool canBeCasted;
    private bool isFired;
    [SerializeField]
    private SpriteRenderer rend;
    private TerrainCollider terrainCollider;
    private SkillBehaviour currentSkill;
    private SkillButton currentSkillButton;
    private Transform hero;
    private float timer = 0;

    public SkillBehaviour CurrentSkill {
        set {
            currentSkill = value;
        }
    }

    public SkillButton CurrentSkillButton {
        set {
            currentSkillButton = value;
        }
    }

    private void OnDisable() {
        currentSkillButton = null;
        CurrentSkill = null;
        isFired = false;
        canBeCasted = false;
    }

    private void Start() {
        hero = PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).transform;
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }

    private void Update() {
        if (isFired) {
            return;
        }
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1) ) {
            gameObject.SetActive(false);
            enabled = false;
            Manager.Instance.SkillOnMouse = false;
            return;
        }
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (terrainCollider.Raycast(ray, out hit, 145f)) {
            transform.position = hit.point;
        }
        if (CheckDistance()) {
            canBeCasted = true;
            rend.material.color = Color.green;
        } else {
            canBeCasted = false;
            transform.localScale = new Vector3(currentSkill.Radius/2, currentSkill.Radius/2, 1);
            rend.material.color = new Color(0.7f, 0.3f, 0.1f);
        }
        if (timer < 0.5f) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (canBeCasted) {
                Fire();
            } else {
                EventLog.Instance.AddAction(LogType.Error, Distance, transform.position);
            }
        }
    }

    private bool CheckDistance() {
        return Vector3.Distance(transform.position, hero.position) <= currentSkill.Range;
    }

    private void Fire() {
        isFired = true;
        currentSkillButton.ExecuteSkill(transform);
        timer = 0;
        enabled = false;
        gameObject.SetActive(false);
    }

}
