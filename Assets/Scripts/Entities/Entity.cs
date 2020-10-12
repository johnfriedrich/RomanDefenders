using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Entity : ParentObject, ISaveLoad {

    public bool MarkedForTraining;
    public bool OnWall;

    [SerializeField]
    protected EntityBehaviour entityBehaviourClass;
    [SerializeField]
    protected NavMeshAgent agent;

    private const int RequiredExperience = 15;

    private int experience;
    private Vector3 finalDestination = Vector3.zero;
    [SerializeField]
    private EntityActionStateEnum entityActionState;
    [SerializeField]
    private EntityBehaviourEnum entityBehaviour;
    [SerializeField]
    private float baseDamage;
    private float originalDamage;
    [SerializeField]
    private float movementSpeed;
    private float originalMoveSpeed;
    [SerializeField]
    private float viewDistance;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private float attackDelay;
    [SerializeField]
    private int foodValue;
    [SerializeField]
    private SoundHolder dieSoundHolder;
    [SerializeField]
    private SoundHolder fightSoundHolder;
    [SerializeField]
    private SoundHolder takeDamageSoundHolder;
    [SerializeField]
    private Sprite uiSprite;
    private float timer = 0f;
    private float timer2 = 0f;
    private GameObject currentEnemy;
    private ParentObject currentEnemyEntity;

    public EntityBehaviour EntityBehaviourClass { get => entityBehaviourClass; }

    public string EntityName { get { return objectName.ToString(); } }

    public int FoodValue {
        get {
            return foodValue;
        }
    }

    public SoundHolder FightSound {
        get {
            return fightSoundHolder;
        }
    }

    public GameObject CurrentEnemy {
        get {
            return currentEnemy;
        }
    }

    public NavMeshAgent Agent {
        get {

            return agent;
        }
    }

    public Sprite UiSprite {
        get {
            return uiSprite;
        }
    }

    public bool IsChasing() {
        return entityActionState == EntityActionStateEnum.Chase;
    }

    public bool IsIdle() {
        return entityActionState == EntityActionStateEnum.Idle;
    }

    public bool IsFighting() {
        return entityActionState == EntityActionStateEnum.Fight;
    }

    public bool IsMoving() {
        return entityActionState == EntityActionStateEnum.Move;
    }

    public void SetAgressive() {
        entityBehaviour = EntityBehaviourEnum.Agressive;
    }

    public void SetPassive() {
        entityBehaviour = EntityBehaviourEnum.Passive;
        ClearTarget();
        Stop();
    }

    public override void OnPointerDown(PointerEventData eventData) {
        if (!OnWall) {
            base.OnPointerDown(eventData);
        }
    }

    public void AddMovementSpeed(int speedToAdd) {
        movementSpeed += speedToAdd;
        agent.speed = movementSpeed;
    }

    public void ReduceMovementSpeed(int speedToRemove) {
        if (speedToRemove >= movementSpeed) {
            return;
        }
        movementSpeed -= speedToRemove;
        agent.speed = movementSpeed;
    }

    public virtual void MoveTo(Vector3 target, bool final) {
        if (final) {
            finalDestination = target;
        }
        ClearTarget();
        agent.isStopped = false;
        SetDestinationWrapper(target);
        entityActionState = EntityActionStateEnum.Move;
        entityBehaviourClass.StartMove();
    }

    public new float CalculateDamage() {
        return baseDamage + Forge.DamageAmount;
    }

    public float GetArmor() {
        return Armor + Forge.ArmorAmount;
    }

    public override bool LevelUp() {
        if (currentLevel < maxLevel) {
            currentLevel++;
            armor++;
            baseDamage++;
            Log.Verbose("{0} leveled from {1} to {2}", objectName.ToString(), (currentLevel - 1), currentLevel);
            infoBar.Show();
            infoBar.UpdateBar();
            EventManager.Instance.EntityLevelUp(this);
            return true;
        }
        Log.Verbose("{0} is already max level: {1} / {2}", objectName.ToString(), currentLevel, maxLevel);
        return true;
    }

    public void SetTarget(GameObject enemyToSetAsTarget) {
        currentEnemy = enemyToSetAsTarget;
        if (enemyToSetAsTarget != null) {
            currentEnemyEntity = currentEnemy.GetComponent<ParentObject>();
        }
    }

    public void SetOwner(OwnerEnum owner) {
        this.owner = owner;
        ClearTarget();
    }

    public void ClearTarget() {
        currentEnemy = null;
        currentEnemyEntity = null;
        entityActionState = EntityActionStateEnum.Idle;
    }

    public void Stop() {
        if (agent.isOnNavMesh) {
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            entityActionState = EntityActionStateEnum.Idle;
            entityBehaviourClass.EndMove();
        }
    }

    public override void TakeDamage(float damage) {
        if (entityState == EntityStateEnum.Dead) {
            return;
        }
        entityActionState = EntityActionStateEnum.Fight;
        float finalDamage = damage - GetArmor();
        if (finalDamage < 0) {
            finalDamage = 0;
        }
        if (!infoBar.IsEnabled()) {
            infoBar.Show();
        }
        Sound.Instance.PlaySoundClipWithSource(takeDamageSoundHolder, audioSource, 0);
        if (finalDamage >= currentHealthPoints) {
            currentHealthPoints = 0;
            finalDamage = currentHealthPoints - finalDamage * -1;
            EventManager.Instance.EntityDamage(this);
            infoBar.UpdateBar();
            Die();
        } else {
            currentHealthPoints -= finalDamage;
            EventManager.Instance.EntityDamage(this);
            infoBar.UpdateBar();
        }
    }

    public new void RepairSelf(int amount) {
        base.RepairSelf(amount);
        if (amount + currentHealthPoints > maxHealthpoints) {
            currentHealthPoints = maxHealthpoints;
            infoBar.Show();
            infoBar.UpdateBar();
            return;
        }
        currentHealthPoints += amount;
        EventManager.Instance.EntityHeal(this);
        infoBar.Show();
        infoBar.UpdateBar();
    }

    public EntitySaveData Save() {
        EntitySaveData saveData = new EntitySaveData(
            owner,
            objectName,
            entityState,
            maxLevel,
            currentLevel,
            maxHealthpoints,
            currentHealthPoints,
            BuildCost,
            BuildTime,
            Armor,
            entityActionState,
            entityBehaviour,
            baseDamage,
            movementSpeed,
            viewDistance,
            attackRange,
            attackDelay,
            foodValue,
            dieSoundHolder,
            fightSoundHolder,
            experience
            );
        return saveData;
    }

    public void Load(EntitySaveData loadedData) {
        owner = loadedData.Owner;
        objectName = loadedData.ObjectName;
        entityState = loadedData.EntityState;
        maxLevel = loadedData.MaxLevel;
        currentLevel = loadedData.CurrentLevel;
        maxHealthpoints = loadedData.MaxHealthpoints;
        currentHealthPoints = loadedData.CurrentHealthPoints;
        buildCost = loadedData.BuildCost;
        buildTime = loadedData.BuildTime;
        armor = loadedData.Armor;
        entityActionState = loadedData.EntityActionState;
        entityBehaviour = loadedData.EntityBehaviour;
        baseDamage = loadedData.BaseDamage;
        movementSpeed = loadedData.MovementSpeed;
        viewDistance = loadedData.ViewDistance;
        attackRange = loadedData.AttackRange;
        attackDelay = loadedData.AttackDelay;
        foodValue = loadedData.FoodValue;
        dieSoundHolder = loadedData.DieSoundHolder;
        fightSoundHolder = loadedData.FightSoundHolder;
        experience = loadedData.Experience;
    }

    protected override void Reset() {
        base.Reset();
        movementSpeed = originalMoveSpeed;
        baseDamage = originalDamage;
        armor = originalArmor;
        entityActionState = EntityActionStateEnum.Idle;
        experience = 0;
        if (agent.hasPath) {
            agent.path = null;
        }
    }

    protected override void OnEnable() {
        base.OnEnable();
        if (!OnWall) {
            agent.enabled = true;
        }
        Reset();
        EventManager.Instance.EntitySpawn(this);
        if (IsPlayer() && !(this is Character) && !(entityBehaviourClass is Catapult)) {
            MeshRenderer.materials[2].color = Manager.Instance.TeamColour;

        }
        if (!IsPlayer()) {
            MeshRenderer.materials[2].color = Manager.Instance.EnemyColor;
        }
        if (IsPlayer()) {
            entityBehaviourClass.SetTeamColor(Manager.Instance.TeamColour);
        } else {
            entityBehaviourClass.SetTeamColor(Manager.Instance.EnemyColor);
        }
        agent.speed = movementSpeed;
    }

    private void Awake() {
        originalMoveSpeed = movementSpeed;
        originalDamage = baseDamage;
        originalArmor = armor;
    }

    private void Die() {
        entityState = EntityStateEnum.Dead;
        OnWall = false;
        agent.enabled = false;
        EventManager.Instance.EntityDeath(this);
        PlayDeathAnimation();
        SetOwner(OwnerEnum.Player);
        BackToPool();
    }

    private Collider[] GetNearEntities(float radius) {
        return Physics.OverlapSphere(transform.position, radius, 256);
    }

    private Collider[] GetNearBuildings(float radius) {
        return Physics.OverlapSphere(transform.position, radius, 512);
    }

    private void ManageExperience() {
        if (currentLevel < maxLevel) {
            experience++;
            if (experience > RequiredExperience) {
                LevelUp();
                experience = 0;
            }
        }
    }

    private bool IsEnemyInAttackRange(Vector3 enemyPosition) {
        float dToTarget = Vector3.Distance(transform.position, enemyPosition);
        return dToTarget <= attackRange;
    }

    private bool Chase(ParentObject enemyToChase) {
        bool reachedTarget = true;
        if (!OnWall && agent.enabled) {
            reachedTarget = IsEnemyInAttackRange(enemyToChase.GetHitpoint(transform.position));
        }
        if (reachedTarget) {
            return true;
        } else {
            agent.isStopped = false;
            entityActionState = EntityActionStateEnum.Chase;
            entityBehaviourClass.StartMove();
            SetDestinationWrapper(enemyToChase.GetHitpoint(transform.position));
            return false;
        }
    }

    private void SetDestinationWrapper(Vector3 position) {
        if (agent.enabled) {
            if (!agent.SetDestination(position)) {
                Debug.Log("Path not found" + objectName.ToString());
            }
        }
    }

    private void PlayDeathAnimation() {
        Color materialColor;
        if (IsPlayer()) {
            materialColor = Manager.Instance.TeamColour;
        } else {
            materialColor = Manager.Instance.EnemyColor;
        }
        PoolHolder.Instance.GetObject(ParentObjectNameEnum.DeathAngel).GetComponent<DeathAngel>().Activate(materialColor, dieSoundHolder, transform.position);
    }

    protected virtual void Update() {
        if (IsMoving() && !agent.enabled) {
            entityBehaviourClass.EndMove();
            entityActionState = EntityActionStateEnum.Idle;
        }
        if (IsMoving() && agent.remainingDistance <= agent.stoppingDistance && agent.velocity == Vector3.zero && !agent.pathPending) {
            if (Vector3.Distance(finalDestination, transform.position) < 20) {
                finalDestination = Vector3.zero;
            }
            entityBehaviourClass.EndMove();
            entityActionState = EntityActionStateEnum.Idle;
            return;
        }
        if (IsAlive()) {
            if (!IsPlayer() && entityActionState == EntityActionStateEnum.Idle && finalDestination != Vector3.zero) {
                MoveTo(finalDestination, false);
            }
            if (currentEnemy != null) {
                if (OnWall) {
                    if (currentEnemy != null && !IsEnemyInAttackRange(currentEnemyEntity.GetHitpoint(transform.position))) {
                        ClearTarget();
                    }
                }
                if (IsFighting() || IsChasing()) {
                    Vector3 hitpoint = currentEnemyEntity.GetHitpoint(transform.position);
                    Vector3 targetPostition = new Vector3(hitpoint.x, transform.position.y, hitpoint.z);
                    transform.LookAt(targetPostition);
                }
            }
            if (timer2 > 0.5f && currentEnemy == null && entityBehaviour != EntityBehaviourEnum.Passive) {
                timer2 = 0;
                //This way we still attack near units first without doing very expensive Distance checks
                SetTarget(EntityUtils.GetNearestEntity(GetNearEntities(viewDistance / 7), this));
                if (currentEnemy == null) {
                    SetTarget(EntityUtils.GetNearestEntity(GetNearEntities(viewDistance / 5), this));
                }
                if (currentEnemy == null) {
                    SetTarget(EntityUtils.GetNearestEntity(GetNearEntities(viewDistance), this));
                }
                if (currentEnemy == null) {
                    SetTarget(EntityUtils.GetNearestBuilding(GetNearBuildings(viewDistance), this));
                }
            }
            timer2 += Time.deltaTime;

            if (currentEnemy != null) {
                if (!IsEnemyInAttackRange(currentEnemyEntity.GetHitpoint(transform.position)) && currentEnemyEntity.IsAlive() && entityBehaviour == EntityBehaviourEnum.Agressive) {
                    if (Chase(currentEnemyEntity)) {
                        Stop();
                    }
                }

                if (timer > attackDelay) {
                    timer = 0;
                    if (currentEnemyEntity.IsAlive() && entityBehaviour != EntityBehaviourEnum.Passive) {
                        if (!currentEnemy.activeInHierarchy) {
                            ClearTarget();
                            return;
                        }
                        if (IsEnemyInAttackRange(currentEnemyEntity.GetHitpoint(transform.position))) {
                            Stop();
                            entityActionState = EntityActionStateEnum.Fight;
                            ManageExperience();
                            entityBehaviourClass.Attack(CalculateDamage(), currentEnemyEntity);
                            if (!currentEnemyEntity.IsAlive()) {
                                ClearTarget();
                                Stop();
                            }
                        } else {
                            if (OnWall) {
                                ClearTarget();
                            }
                        }
                    } else if (!currentEnemyEntity.IsAlive() || !IsAlive()) {
                        ClearTarget();
                        Stop();
                    }
                }
                timer += Time.deltaTime;
            }
        }
    }

}
