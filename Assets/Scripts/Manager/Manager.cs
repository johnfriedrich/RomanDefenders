﻿using Buildings.Behaviour;
using Entities;
using FoW;
using ObjectManagement;
using Parent;
using Sound;
using UI.CharacterEditor;
using UI.HUD;
using UI.IngameMenu;
using UI.MainMenu;
using UI.Options;
using UnityEngine;
using LogType = UI.HUD.LogType;

namespace Manager {
    public class Manager : MonoBehaviour {

        public static Manager Instance { get; private set; }
        public const int MaxFood = 500;
        public const int MinFood = 200;

        public readonly string CurrencyName = "Mana";
        public readonly string FoodName = "Farm places";

        public GameObject BuildingPrefabOnMouse;
        public bool SkillOnMouse;
        public int MaxWaveNumber = 0;

        [SerializeField]
        private Texture2D cursorTexture;
        [SerializeField]
        private int mana = 0;
        private int defaultMana;
        [SerializeField]
        private Sprite manaSprite;
        [SerializeField]
        private Sprite entityLevelStarSprite;
        [SerializeField]
        private int currentFood = MinFood;
        [SerializeField]
        private GameObject enemyTarget;
        [SerializeField]
        private GameObject enemySpawn;
        [SerializeField]
        private MainMenu mainMenu;
        [SerializeField]
        private GameObject beforeGameScreen;
        [SerializeField]
        private IngameMenu ingameMenu;
        [SerializeField]
        private Tutorial tutorial;
        [SerializeField]
        private WavesDefeated wavesDefeated;
        [SerializeField]
        private GameWin gameWin;
        [SerializeField]
        private GameLost gameLost;
        [SerializeField]
        private Cheats cheats;
        [SerializeField]
        private Options options;
        [SerializeField]
        private Help help;
        [SerializeField]
        private GameObject game;
        [SerializeField]
        private CharacterEditor characterEditor;
        [SerializeField]
        private PlayerSettings playerSettings;
        [SerializeField]
        private GameObject skillTargeter;
        [SerializeField]
        private Color enemyColor;
        [SerializeField]
        private GameObject menuCamera;

        public int CurrentFood {
            get {
                if (currentFood >= MaxFood) {
                    return MaxFood;
                } else {
                    return currentFood;
                };
            }
        }

        public int CurrentEntityCount { get; private set; } = 0;

        public MainMenu MainMenu => mainMenu;

        public Options Options => options;

        public int Mana => mana;

        public CharacterEditor CharacterEditor {
            get {
                return characterEditor;
            }
        }

        public Color TeamColour {
            get => playerSettings.teamColour;

            set => playerSettings.teamColour = value;
        }

        public Sprite ManaSprite => manaSprite;

        public bool IsPaused { get; internal set; }

        public GameObject EnemyTarget => enemyTarget;

        public GameObject EnemySpawn => enemySpawn;

        public Sprite EntityLevelStarSprite => entityLevelStarSprite;

        public GameObject BeforeGameScreen => beforeGameScreen;

        public bool IsGameRunning { get; private set; }

        public Color EnemyColor => enemyColor;

        public GameObject SkillTargeter => skillTargeter;

        public Help Help => help;

        public WavesDefeated WavesDefeated => wavesDefeated;

        public Texture2D CursorTexture { get => cursorTexture;  }

        public void AddFood(int foodToAdd) {
            currentFood += foodToAdd;
        }

        public void RemoveFood(int foodToRemove) {
            if (CurrentFood - foodToRemove < MinFood) {
                currentFood = MinFood;
            } else {
                currentFood -= foodToRemove;
            }
        }

        public bool HasEnoughFood(int foodAmountToCheck) {
            return foodAmountToCheck + CurrentEntityCount <= currentFood;
        }

        public void AddMana(int manaToAdd) {
            mana += manaToAdd;
            EventManager.Instance.RefreshUI();
        }

        private void RemoveMana(int manaToRemove) {
            mana -= manaToRemove;
            EventManager.Instance.RefreshUI();
        }

        public bool HasEnoughMana(float amountToCheck) {
            return amountToCheck <= Mana;
        }

        private void SetGameRunning() {
            IsGameRunning = true;
        }

        private void WinGame() {
            SetGameStopped();
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Pling);
            gameWin.Show();
        }

        private void LooseGame() {
            SetGameStopped();
            gameLost.Show();
        }

        private void SetGameStopped() {
            IsGameRunning = false;
        }

        private void Awake() {
            Instance = this;
            game.SetActive(false);
            beforeGameScreen.SetActive(true);
            EventManager.Instance.OnEntitySpawnEvent += EntitySpawnActions;
            EventManager.Instance.OnEntityTrainEvent += EntityTrainActions;
            EventManager.Instance.OnResearchStartedEvent += ResearchStartedActions;
            EventManager.Instance.OnBuildingStartedEvent += BuildingStartedActions;
            EventManager.Instance.OnBuildingUpgradeStartedEvent += BuildingUpgradeStartedActions;
            EventManager.Instance.OnBuildingBuiltEvent += BuildingBuiltActions;
            EventManager.Instance.OnStartGameEvent += SetGameRunning;
            EventManager.Instance.OnGameOverPostEvent += LooseGame;
            EventManager.Instance.OnWinGameEvent += WinGame;
            EventManager.Instance.OnQuitGameEvent += SetGameStopped;
            EventManager.Instance.OnBuildingRemovedEvent += BuildingRemovedActions;
            EventManager.Instance.OnEntityDeathEvent += EntityDeathActions;
            EventManager.Instance.OnStartGamePreEvent += EnableGameWorld;
            EventManager.Instance.OnStartGamePostEvent += tutorial.Show;
            EventManager.Instance.OnGameResetEvent += ResetOptions;
            EventManager.Instance.OnAllWavesFinishedEvent += wavesDefeated.Show;
        }

        private void Start() {
            if (playerSettings == null) {
                playerSettings = new PlayerSettings();
            }
            defaultMana = mana;
        }

        private void ResetOptions() {
            EventManager.Instance.Deselect();
            FogOfWar.instances[0].SetAll(255);
            mana = defaultMana;
            CurrentEntityCount = 0;
            currentFood = 200;
            FlyCamera.Instance.JumpToTarget(enemyTarget.transform.position);
        }

        private void EnableGameWorld() {
            menuCamera.SetActive(false);
            game.SetActive(true);
        }

        private void EntitySpawnActions(Entity spawnedEntity) { }

        private void EntityDeathActions(Entity deadEntity) {
            if (deadEntity.IsPlayer()) {
                RemoveEntity(deadEntity.FoodValue);
            }
        }

        private void BuildingStartedActions(ParentBuilding parentBuilding) {
            RemoveMana(parentBuilding.BuildCost);
        }

        private void BuildingUpgradeStartedActions(ParentBuilding parentBuilding) {
            RemoveMana(parentBuilding.UpgradeCost);
        }

        private void ResearchStartedActions(Forge forge) {
            RemoveMana(forge.ResearchCost);
        }

        private void EntityTrainActions(Entity entityInTraining) {
            AddEntity(entityInTraining.FoodValue);
            RemoveMana(entityInTraining.BuildCost);
        }

        private void BuildingBuiltActions(ParentBuilding building) {
            building.Behaviour.OnBuildingBuilt(building);
        }

        private void BuildingRemovedActions(ParentBuilding building, bool byEnemy) {
            if (!byEnemy) {
                AddMana(building.BuildCost / 2);
                EventLog.Instance.AddAction(LogType.Destroyed, building.FriendlyName + " removed", building.gameObject.transform.position);
            } else {
                EventLog.Instance.AddAction(LogType.Destroyed, building.FriendlyName + " was destroyed!", building.gameObject.transform.position);
            }
            building.Behaviour.OnBuildingRemoved(building);
        }

        private void AddEntity(int entityToAdd) {
            CurrentEntityCount += entityToAdd;
        }

        private void RemoveEntity(int entityToRemove) {
            CurrentEntityCount -= entityToRemove;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.C) && IsGameRunning) {
                if (cheats.isActiveAndEnabled) {
                    cheats.Hide();
                } else {
                    cheats.Show();
                }
            }
            if (!IsGameRunning || BuildingPrefabOnMouse != null) {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape) && PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>().Casting && PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>().IsSelected()) {
                PoolHolder.Instance.GetObjectActive(ParentObjectNameEnum.Character).GetComponent<Character>().CancelCast();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape) && !ingameMenu.isActiveAndEnabled) {
                ingameMenu.Show();
                return;
            }
            if (Input.GetKeyDown(KeyCode.Escape) && ingameMenu.isActiveAndEnabled) {
                ingameMenu.Hide();
            }
        }

    }
}
