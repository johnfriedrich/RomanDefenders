using Buildings;
using FoW;
using UI.HUD;
using UnityEngine;
using Utils;
using LogType = UI.HUD.LogType;

public class BuildingPlacement : MonoBehaviour {

    private const string OverlapObject = "You cannot build here because it overlaps with another building or an entity";
    private const string InFOW = "You cannot build here because it's hidden in the fog";

    private string reason = "You cannot build here. Reason: Unknown.";
    private bool canBePlaced;
    private bool isPlaced;
    private Renderer rend;
    private BoxCollider boxCollider;
    private TerrainCollider terrainCollider;

    private void Start() {
        rend = GetComponent<ParentBuilding>().MeshRenderer;
        boxCollider = GetComponent<BoxCollider>();
        terrainCollider = Terrain.activeTerrain.GetComponent<TerrainCollider>();
    }

    private void Update() {
        if (Manager.Manager.Instance.BuildingPrefabOnMouse == null || isPlaced) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Mouse1) || !Manager.Manager.Instance.HasEnoughMana(Manager.Manager.Instance.BuildingPrefabOnMouse.GetComponent<ParentBuilding>().BuildCost)) {
            Manager.Manager.Instance.BuildingPrefabOnMouse.SetActive(false);
            Manager.Manager.Instance.BuildingPrefabOnMouse = null;
            Destroy(this);
            return;
        }
        RaycastHit hit;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (terrainCollider.Raycast(ray, out hit, 145f)) {
            Manager.Manager.Instance.BuildingPrefabOnMouse.transform.position = hit.point;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                transform.Rotate(Vector3.up * 2f, Space.Self);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                transform.Rotate(Vector3.down * 2f, Space.Self);
            }
        }
        Collider[] colliderList = Physics.OverlapBox(boxCollider.bounds.center, boxCollider.bounds.extents, Quaternion.identity);
        bool containsObject = false;
        foreach (Collider collider in colliderList) {
            if ((collider.CompareTag(TagData.Building) || collider.CompareTag(TagData.Entity) || collider.CompareTag(TagData.Tree)) && collider.gameObject != gameObject) {
                containsObject = true;
            }
        }
        if (!containsObject && !CheckFOW()) {
            canBePlaced = true;
            rend.material.color = Color.white;
        } else {
            canBePlaced = false;
            if (containsObject) {
                reason = OverlapObject;
            } else if (CheckFOW()) {
                reason = InFOW;
            }
            rend.material.color = new Color(0.7f, 0.3f, 0.1f);
        }
    }

    private bool CheckFOW() {
        return FogOfWar.instances[0].IsInCompleteFog(transform.position);
    }

    private void OnMouseDown() {
        if (canBePlaced) {
            Place();
        } else {
            EventLog.Instance.AddAction(LogType.Error, reason, transform.position);
        }
    }

    private void Place() {
        isPlaced = true;
        Manager.Manager.Instance.BuildingPrefabOnMouse = null;
        gameObject.GetComponent<TerrainLeveler>().enabled = true;
        gameObject.GetComponent<Constructing>().enabled = true;
        gameObject.GetComponent<FogOfWarUnit>().enabled = true;
        Destroy(this);
    }

}

