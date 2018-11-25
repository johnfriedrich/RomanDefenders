using FoW;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ParentBuilding : ParentObject, IRepairable {

    [SerializeField]
    private string upgradeDescription;
    [SerializeField]
    private BuildingBehaviour behaviour;
    [SerializeField]
    private Sprite iconSprite;
    [SerializeField]
    private int upgradeCost;
    private bool isUpgrading;
    [SerializeField]
    private float upgradeTime;
    [SerializeField]
    private NavMeshObstacle navMeshObstacle;
    [SerializeField]
    private ButtonCooldown upgradeCooldown;
    private Texture normalAlbedo;
    private Texture mormalNormalMap;
    [SerializeField]
    private Texture crackedAlbedo;
    [SerializeField]
    private Texture crackedNormalMap;
    [SerializeField]
    private SoundHolder destroySoundHolder;
    [SerializeField]
    private SoundHolder upgradeCompleteSoundHolder;

    private enum TextureState {
        Normal,
        Cracked
    }

    public int UpgradeCost {
        get {
            return upgradeCost;
        }
    }

    public bool IsUpgrading {
        get {
            return isUpgrading;
        }
    }

    public BuildingBehaviour Behaviour {
        get {
            return behaviour;
        }
    }

    public Sprite IconSprite {
        get {
            return iconSprite;
        }
    }

    public NavMeshObstacle NavMeshObstacle {
        get {
            return navMeshObstacle;
        }
    }

    public ButtonCooldown UpgradeCooldown {
        get {
            return upgradeCooldown;
        }
    }

    public string UpgradeDescription {
        get {
            return upgradeDescription;
        }
    }

    public void SpawnDustParticles(float time) {
        StartCoroutine(DustParticlesForTime(time));
    }

    public void Shake() {
        StartCoroutine(ShakeObject());
    }

    public override void TakeDamage(float damage) {
        float finalDamage = damage - Armor;
        if (finalDamage < 0) {
            finalDamage = 0;
        }
        if (!infoBar.IsEnabled()) {
            infoBar.Show();
        }
        if (currentHealthPoints * 2 <= maxHealthpoints && MeshRenderer.material.mainTexture != crackedAlbedo) {
            if (crackedAlbedo != null) {
                SetTextures(TextureState.Cracked);
            }
        }
        if (finalDamage >= currentHealthPoints) {
            currentHealthPoints = 0;
            finalDamage = currentHealthPoints - finalDamage * -1;
            EventManager.Instance.BuildingDamage(this);
            infoBar.UpdateBar();
            Die();
            Destroy(true);
        } else {
            currentHealthPoints -= finalDamage;
            EventManager.Instance.BuildingDamage(this);
            infoBar.UpdateBar();
        }
    }

    public void Destroy(bool enemy) {
        if (!enemy && !behaviour.Destroy()) {
            return;
        }
        if (isUpgrading && !enemy) {
            EventLog.Instance.AddAction(LogType.Error, Messages.CantDestroyBuildingWhileUpgrading, transform.position);
            return;
        }
        EventManager.Instance.BuildingRemoved(this, enemy);
        PoolHolder.Instance.GetObject(ParentObjectNameEnum.DeathObject).GetComponent<DeathObject>().Activate(destroySoundHolder, transform.position);
        navMeshObstacle.enabled = false;
        BackToPool();
        SetTextures(TextureState.Normal);
        gameObject.GetComponent<FogOfWarUnit>().enabled = false;
        if (!IsPlayer()) {
            owner = OwnerEnum.Player;
            Destroy(gameObject, 5);
        }
    }

    public override bool LevelUp() {
        bool canBeUpgraded = false;
        //canBeUpgraded = behaviour.LevelUp();
        if (currentLevel >= maxLevel) {
            EventLog.Instance.AddAction(LogType.Error, objectName.ToString() + " is already max level: " + currentLevel + "/" + maxLevel, transform.position);
            return canBeUpgraded;
        } else if (!Manager.Instance.HasEnoughMana(upgradeCost)) {
            EventLog.Instance.AddAction(LogType.Error, "You need " + upgradeCost + " " + Manager.Instance.CurrencyName + " to upgrade " + objectName, transform.position);
            return canBeUpgraded;
        } else if (isUpgrading) {
            EventLog.Instance.AddAction(LogType.Error, Messages.AlreadyUpgradingBuilding, transform.position);
            return canBeUpgraded;
        } else if (behaviour is Forge) {
            Forge forge = (Forge)behaviour;
            if (forge.IsDoingResearch) {
                EventLog.Instance.AddAction(LogType.Error, Messages.CantUpgradeWhileResearching, transform.position);
                return canBeUpgraded;
            }
        } else if (behaviour is Barracks) {
            Barracks barracks = (Barracks)behaviour;
            if (barracks.Alreadytraining) {
                EventLog.Instance.AddAction(LogType.Error, Messages.CantUpgradingWhileTraining, transform.position);
                return canBeUpgraded;
            }
        }
        canBeUpgraded = true;
        SpawnDustParticles(upgradeTime);
        StartCoroutine(DoUpgrade(upgradeTime));
        return canBeUpgraded;
    }

    protected override void OnEnable() {
        base.OnEnable();
        Reset();
        EventManager.Instance.BuildingBuilt(this);
    }

    private void Start() {
        normalAlbedo = MeshRenderer.material.mainTexture;
        mormalNormalMap = MeshRenderer.material.GetTexture("_BumpMap");
        if (crackedAlbedo != null && crackedAlbedo == normalAlbedo) {
            SetTextures(TextureState.Normal);
        }
    }

    private void SetTextures(TextureState state) {
        if (state == TextureState.Normal) {
            MeshRenderer.material.mainTexture = normalAlbedo;
            MeshRenderer.material.SetTexture("_BumpMap", mormalNormalMap);
        } else {
            MeshRenderer.material.mainTexture = crackedAlbedo;
            MeshRenderer.material.SetTexture("_BumpMap", crackedNormalMap);
        }
    }

    private void OnDisable() {
        enabled = false;
        infoBar.Hide();
    }

    private IEnumerator ShakeObject() {
        Vector3 oldScale = transform.localScale;
        Vector3 offset = new Vector3(0.05f, 0, 0.05f);
        transform.localScale += offset;
        yield return new WaitForSecondsRealtime(0.1f);
        transform.localScale -= offset;
    }

    private IEnumerator DustParticlesForTime(float time) {
        GameObject dustParticles = PoolHolder.Instance.GetObject(ParentObjectNameEnum.UpgradeParticles);
        ParticleSystem particles = dustParticles.GetComponent<ParticleSystem>();
        ParticleSystem.ShapeModule shape = particles.shape;
        shape.meshRenderer = (MeshRenderer)mainRenderer;
        dustParticles.SetActive(true);
        yield return new WaitForSeconds(time);
        shape.meshRenderer = null;
        dustParticles.SetActive(false);
    }

    private IEnumerator DoUpgrade(float time) {
        EventManager.Instance.BuildingUpgradeStarted(this);
        upgradeCooldown.SetCooldown(upgradeTime);
        isUpgrading = true;
        yield return new WaitForSeconds(time);
        isUpgrading = false;
        currentLevel++;
        Sound.Instance.PlaySoundClipWithSource(upgradeCompleteSoundHolder, audioSource, 0);
        behaviour.LevelUp();
        base.LevelUp();
        EventLog.Instance.AddAction(LogType.Upgraded, objectName.ToString() + " leveled from " + (currentLevel - 1) + " to " + currentLevel, transform.position);
    }

    private void Die() {
        entityState = EntityStateEnum.Dead;
    }

}
