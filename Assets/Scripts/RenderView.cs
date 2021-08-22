using Entities;
using Manager;
using Parent;
using UI.CharacterEditor;
using UnityEngine;

public class RenderView : MonoBehaviour {

    public static RenderView Instance { get; private set; }

    [SerializeField]
    private GameObject characterPrefabinternal;
    [SerializeField]
    private GameObject swordsmanPrefabInternal;
    [SerializeField]
    private GameObject bowmanPrefabInternal;
    [SerializeField]
    private GameObject horsemanPrefabInternal;
    [SerializeField]
    private GameObject settlerPrefabInternal;
    [SerializeField]
    private GameObject catapultPrefabInternal;
    [SerializeField]
    private GameObject forgePrefabInternal;
    [SerializeField]
    private GameObject mainhousePrefabInternal;
    [SerializeField]
    private GameObject farmPrefabInternal;
    [SerializeField]
    private GameObject minePrefabInternal;
    [SerializeField]
    private GameObject barracksPrefabInternal;
    [SerializeField]
    private GameObject woodenWallPrefabInternal;
    [SerializeField]
    private GameObject stoneWallPrefabInternal;
    [SerializeField]
    private GameObject stoneTowerPrefabInternal;

    public GameObject GetRenderObject(ParentObjectNameEnum type) {
        switch (type) {
            case ParentObjectNameEnum.Character:
                return characterPrefabinternal;
            case ParentObjectNameEnum.Swordsman:
                return swordsmanPrefabInternal;
            case ParentObjectNameEnum.Bowman:
                return bowmanPrefabInternal;
            case ParentObjectNameEnum.Horseman:
                return horsemanPrefabInternal;
            case ParentObjectNameEnum.Settler:
                return settlerPrefabInternal;
            case ParentObjectNameEnum.Catapult:
                return catapultPrefabInternal;
            case ParentObjectNameEnum.Forge:
                return forgePrefabInternal;
            case ParentObjectNameEnum.Mainhouse:
                return mainhousePrefabInternal;
            case ParentObjectNameEnum.Farm:
                return farmPrefabInternal;
            case ParentObjectNameEnum.Mine:
                return minePrefabInternal;
            case ParentObjectNameEnum.Barracks:
                return barracksPrefabInternal;
            case ParentObjectNameEnum.WoodWall:
                return woodenWallPrefabInternal;
            case ParentObjectNameEnum.StoneWall:
                return stoneWallPrefabInternal;
            case ParentObjectNameEnum.StoneTower:
                return stoneTowerPrefabInternal;
            default:
                return null;
        }
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        EventManager.Instance.OnStartGamePostEvent += LoadColors;
        LoadColors();
    }

    private void LoadColors() {
        characterPrefabinternal.GetComponent<Character>().Load(CharacterEditor.Instance.CharacterSaveData);
        swordsmanPrefabInternal.GetComponent<Entity>().MeshRenderer.materials[2].color = Manager.Manager.Instance.TeamColour;
        bowmanPrefabInternal.GetComponent<Entity>().MeshRenderer.materials[2].color = Manager.Manager.Instance.TeamColour;
        horsemanPrefabInternal.GetComponent<Entity>().MeshRenderer.materials[2].color = Manager.Manager.Instance.TeamColour;
        settlerPrefabInternal.GetComponent<Entity>().MeshRenderer.materials[2].color = Manager.Manager.Instance.TeamColour;
    }

}
