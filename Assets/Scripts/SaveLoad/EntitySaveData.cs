using System;
using Entities;
using Parent;
using Sound;
using UnityEngine;

[Serializable]
public class EntitySaveData {

    [SerializeField]
    private OwnerEnum owner;
    [SerializeField]
    private ParentObjectNameEnum objectName;
    [SerializeField]
    private EntityStateEnum entityState;
    [SerializeField]
    private int maxLevel;
    [SerializeField]
    private int currentLevel;
    [SerializeField]
    private float maxHealthpoints;
    [SerializeField]
    private float currentHealthPoints;
    [SerializeField]
    private int buildCost;
    [SerializeField]
    private int buildTime;
    [SerializeField]
    private float armor;
    [SerializeField]
    private EntityActionStateEnum entityActionState;
    [SerializeField]
    private EntityBehaviourEnum entityBehaviour;
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float movementSpeed;
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
    private int experience;

    public EntitySaveData() { }

    public EntitySaveData(OwnerEnum owner, ParentObjectNameEnum objectName, EntityStateEnum entityState, int maxLevel, int currentLevel, float maxHealthpoints, float currentHealthPoints, int buildCost, int buildTime, float armor, EntityActionStateEnum entityActionState, EntityBehaviourEnum entityBehaviour, float baseDamage, float movementSpeed, float viewDistance, float attackRange, float attackDelay, int foodValue, SoundHolder dieSoundHolder, SoundHolder fightSoundHolder, int experience) {
        this.owner = owner;
        this.objectName = objectName;
        this.entityState = entityState;
        this.maxLevel = maxLevel;
        this.currentLevel = currentLevel;
        this.maxHealthpoints = maxHealthpoints;
        this.currentHealthPoints = currentHealthPoints;
        this.buildCost = buildCost;
        this.buildTime = buildTime;
        this.armor = armor;
        this.entityActionState = entityActionState;
        this.entityBehaviour = entityBehaviour;
        this.baseDamage = baseDamage;
        this.movementSpeed = movementSpeed;
        this.viewDistance = viewDistance;
        this.attackRange = attackRange;
        this.attackDelay = attackDelay;
        this.foodValue = foodValue;
        this.dieSoundHolder = dieSoundHolder;
        this.fightSoundHolder = fightSoundHolder;
        this.experience = experience;
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

    public EntityStateEnum EntityState {
        get {
            return entityState;
        }
    }

    public int MaxLevel {
        get {
            return maxLevel;
        }
    }

    public int CurrentLevel {
        get {
            return currentLevel;
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

    public EntityActionStateEnum EntityActionState {
        get {
            return entityActionState;
        }
    }

    public EntityBehaviourEnum EntityBehaviour {
        get {
            return entityBehaviour;
        }
    }

    public float BaseDamage {
        get {
            return baseDamage;
        }

    }

    public float MovementSpeed {
        get {
            return movementSpeed;
        }
    }

    public float ViewDistance {
        get {
            return viewDistance;
        }
    }

    public float AttackRange {
        get {
            return attackRange;
        }
    }

    public float AttackDelay {
        get {
            return attackDelay;
        }
    }

    public int FoodValue {
        get {
            return foodValue;
        }
    }

    public SoundHolder DieSoundHolder {
        get {
            return dieSoundHolder;
        }
    }

    public SoundHolder FightSoundHolder {
        get {
            return fightSoundHolder;
        }
    }

    public int Experience {
        get {
            return experience;
        }
    }

}
