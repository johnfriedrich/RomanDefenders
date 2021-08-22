using System.Collections.Generic;
using Buildings.Behaviour;
using Entities;
using Entities.Skills;
using Parent;

namespace Manager {
    public class EventManager {

        public static EventManager Instance => _instance ??= new EventManager();

        private static EventManager _instance;

        public delegate void DelSimple();
        public delegate void DelBool(bool value);
        public delegate void DelEntity(Entity entity);
        public delegate void DelForge(Forge forge);
        public delegate void DelEntityBuildingBehaviour(Entity entity, BuildingBehaviour buildingBehaviour);
        public delegate void DelStringList(List<string> stringList);
        public delegate void DelParentBuilding(ParentBuilding building);
        public delegate void DelParentBuildingEnemy(ParentBuilding building, bool byEnemy);
        public delegate void DelParentObject(ParentObject parentObject);
        public delegate void DelSelectionList(List<ParentObject> entities);
        public delegate void DelSkillBehaviour(SkillBehaviour canceledSkill);

        public event DelEntity OnEntityDeathEvent;
        public event DelSelectionList OnParentObjectSelectedEvent;
        public event DelSimple OnDeselectEvent;
        public event DelEntity OnEntityDamageEvent;
        public event DelEntity OnEntityHealEvent;
        public event DelEntity OnEntitySpawnEvent;
        public event DelEntity OnEntityLevelUpEvent;
        public event DelEntity OnEntityTrainEvent;
        public event DelEntityBuildingBehaviour OnEntityTrainFinishedEvent;
        public event DelForge OnResearchStartedEvent;
        public event DelSimple OnRefreshUIEvent;
        public event DelParentBuilding OnBuildingBuiltEvent;
        public event DelParentBuilding OnBuildingDamageEvent;
        public event DelParentBuilding OnBuildingStartedEvent;
        public event DelParentBuilding OnBuildingUpgradeStartedEvent;
        public event DelParentBuildingEnemy OnBuildingRemovedEvent;
        public event DelSimple OnWaveFinishEvent;
        public event DelSimple OnWaveStartEvent;
        public event DelSimple OnAllWavesFinishedEvent;
        public event DelSkillBehaviour OnCancelSkillEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameResetPostEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameResetPreEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameResetEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameOverPreEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameOverEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler verloren hat
        /// </summary>
        public event DelSimple OnGameOverPostEvent;

        /// <summary>
        /// Event wird aufgerufen, bevor das Spiel gestartet wird
        /// </summary>
        public event DelSimple OnStartGamePreEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn das Spiel gestartet wird
        /// </summary>
        public event DelSimple OnStartGameEvent;

        /// <summary>
        /// Event wird aufgerufen, nachdem das Spiel gestartet wurde
        /// </summary>
        public event DelSimple OnStartGamePostEvent;

        /// <summary>
        /// Event wird aufgerufen, nachdem das Spiel pausiert wurde
        /// </summary>
        public event DelBool OnPauseGameEvent;

        /// <summary>
        /// Event wird aufgerufen, nachdem das Spiel weitergeführt wurde
        /// </summary>
        public event DelSimple OnResumeGameEvent;

        /// <summary>
        /// Event wird aufgerufen, nachdem das Spiel gewonnen wurde
        /// </summary>
        public event DelSimple OnWinGameEvent;

        /// <summary>
        /// Event wird aufgerufen, wenn der Spieler das Spiel verlässt
        /// </summary>
        public event DelSimple OnQuitGameEvent;

        private void EntityDeathEvent(Entity deadEntity) {
            OnEntityDeathEvent?.Invoke(deadEntity);
        }

        public void EntityDeath(Entity deadEntity) {
            OnEntityDeathEvent?.Invoke(deadEntity);
        }

        private void ParentObjectSelectedEvent(List<ParentObject> selectedObjects) {
            OnParentObjectSelectedEvent?.Invoke(selectedObjects);
        }

        public void ParentObjectSelected(List<ParentObject> selectedObjects) {
            OnParentObjectSelectedEvent?.Invoke(selectedObjects);
        }

        private void DeselectEvent() {
            OnDeselectEvent?.Invoke();
        }

        public void Deselect() {
            OnDeselectEvent?.Invoke();
        }

        private void EntityDamageEvent(Entity damagedEntity) {
            OnEntityDamageEvent?.Invoke(damagedEntity);
        }

        public void EntityDamage(Entity damagedEntity) {
            OnEntityDamageEvent?.Invoke(damagedEntity);
        }

        private void EntityHealEvent(Entity healedEntity) {
            OnEntityHealEvent?.Invoke(healedEntity);
        }

        public void EntityHeal(Entity healedEntity) {
            OnEntityHealEvent?.Invoke(healedEntity);
        }

        private void EntitySpawnEvent(Entity spawnedEntity) {
            OnEntitySpawnEvent?.Invoke(spawnedEntity);
        }

        public void EntitySpawn(Entity spawnedEntity) {
            OnEntitySpawnEvent?.Invoke(spawnedEntity);
        }

        private void EntityLevelUpEvent(Entity leveledEntity) {
            OnEntityLevelUpEvent?.Invoke(leveledEntity);
        }

        public void EntityLevelUp(Entity leveledEntity) {
            OnEntityLevelUpEvent?.Invoke(leveledEntity);
        }

        private void EntityTrainEvent(Entity entityInTraining) {
            OnEntityTrainEvent?.Invoke(entityInTraining);
        }

        public void EntityTrain(Entity entityInTraining) {
            OnEntityTrainEvent?.Invoke(entityInTraining);
        }

        private void EntityTrainFinishedEvent(Entity entityInTraining, BuildingBehaviour buildingBehaviour) {
            OnEntityTrainFinishedEvent?.Invoke(entityInTraining, buildingBehaviour);
        }

        public void EntityTrainFinished(Entity entityInTraining, BuildingBehaviour buildingBehaviour) {
            OnEntityTrainFinishedEvent?.Invoke(entityInTraining, buildingBehaviour);
        }

        private void RefreshUIEvent() {
            OnRefreshUIEvent?.Invoke();
        }

        public void RefreshUI() {
            OnRefreshUIEvent?.Invoke();
        }

        private void BuildingBuiltEvent(ParentBuilding building) {
            OnBuildingBuiltEvent?.Invoke(building);
        }

        public void BuildingBuilt(ParentBuilding building) {
            OnBuildingBuiltEvent?.Invoke(building);
        }

        private void BuildingDamageEvent(ParentBuilding damagedBuilding) {
            OnBuildingDamageEvent?.Invoke(damagedBuilding);
        }

        public void BuildingDamage(ParentBuilding damagedBuilding) {
            OnBuildingDamageEvent?.Invoke(damagedBuilding);
        }

        private void BuildingStartedEvent(ParentBuilding building) {
            OnBuildingStartedEvent?.Invoke(building);
        }

        public void BuildingStarted(ParentBuilding building) {
            OnBuildingStartedEvent?.Invoke(building);
        }

        private void BuildingUpgradeStartedEvent(ParentBuilding building) {
            OnBuildingUpgradeStartedEvent?.Invoke(building);
        }

        public void BuildingUpgradeStarted(ParentBuilding building) {
            OnBuildingUpgradeStartedEvent?.Invoke(building);
        }

        private void ResearchStartedEvent(Forge forge) {
            OnResearchStartedEvent?.Invoke(forge);
        }

        public void ResearchStarted(Forge forge) {
            OnResearchStartedEvent?.Invoke(forge);
        }

        private void BuildingRemovedEvent(ParentBuilding building, bool byEnemy) {
            OnBuildingRemovedEvent?.Invoke(building, byEnemy);
        }

        public void BuildingRemoved(ParentBuilding building, bool byEnemy) {
            OnBuildingRemovedEvent?.Invoke(building, byEnemy);
        }

        /// <summary>
        /// Methode die vor dem ausführen des GameResetEvent prüft ob sich Methoden auf diese Events angemeldet haben.
        /// </summary>
        private void GameResetEventChain() {
            OnGameResetPreEvent?.Invoke();
            OnGameResetEvent?.Invoke();
            OnGameResetPostEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn der Spieler verloren hat.
        /// </summary>
        public void GameReset() {
            GameResetEventChain();
        }

        /// <summary>
        /// Methode die vor dem ausführen des GameOverEvent prüft ob sich Methoden auf diese Events angemeldet haben.
        /// </summary>
        private void GameOverEventChain() {
            OnGameOverPreEvent?.Invoke();
            OnGameOverEvent?.Invoke();
            OnGameOverPostEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn der Spieler verloren hat.
        /// </summary>
        public void GameOver() {
            GameOverEventChain();
        }

        /// <summary>
        /// Methode die vor dem ausführen des StartGameEvent prüft ob sich Methoden auf diesen Events angemeldet haben.
        /// </summary>
        private void StartGameEventChain() {
            OnStartGamePreEvent?.Invoke();
            OnStartGameEvent?.Invoke();
            OnStartGamePostEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn das Spiel gestartet wirdt.
        /// </summary>
        public void StartGame() {
            StartGameEventChain();
        }

        /// <summary>
        /// Methode die vor dem ausführen des PauseGameEvent prüft ob sich Methoden auf diesen Events angemeldet haben.
        /// </summary>
        private void PauseGameEvent(bool pauseValue) {
            OnPauseGameEvent?.Invoke(pauseValue);
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn das Spiel pausiert wird.
        /// </summary>
        public void PauseGame(bool pauseValue) {
            PauseGameEvent(pauseValue);
        }

        /// <summary>
        /// Methode die vor dem ausführen des ResumeGameEvent prüft ob sich Methoden auf diesen Events angemeldet haben.
        /// </summary>
        private void ResumeGameEvent() {
            OnResumeGameEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn das Spiel weitergeführt wird.
        /// </summary>
        public void ResumeGame() {
            ResumeGameEvent();
        }

        /// <summary>
        /// Methode die vor dem ausführen des WinGamevent prüft ob sich Methoden auf diesen Events angemeldet haben.
        /// </summary>
        private void WinGameEvent() {
            OnWinGameEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn das Spiel gewonnen wurde.
        /// </summary>
        public void WinGame() {
            WinGameEvent();
        }

        /// <summary>
        /// Methode die vor dem ausführen des QuitGameEvent prüft ob sich Methoden auf diesen Events angemeldet haben.
        /// </summary>
        private void QuitGameEvent() {
            OnQuitGameEvent?.Invoke();
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, wenn der Spieler das Spiel verlassen hat.
        /// </summary>
        public void QuitGame() {
            QuitGameEvent();
        }

        public void WaveFinish() {
            WaveFinishEvent();
        }

        private void WaveFinishEvent() {
            OnWaveFinishEvent?.Invoke();
        }

        public void WaveStart() {
            WaveStartEvent();
        }

        private void WaveStartEvent() {
            OnWaveStartEvent?.Invoke();
        }

        public void AllWavesFinished() {
            AllWavesFinishedEvent();
        }

        private void AllWavesFinishedEvent() {
            OnAllWavesFinishedEvent?.Invoke();
        }

        public void CancelSkill(SkillBehaviour canceledSkill) {
            CancelSkillEvent(canceledSkill);
        }

        private void CancelSkillEvent(SkillBehaviour canceledSkill) {
            OnCancelSkillEvent?.Invoke(canceledSkill);
        }

    }
}
