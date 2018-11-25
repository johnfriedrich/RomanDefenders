using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Mine : BuildingBehaviour {

    [SerializeField]
    private float delayBetweenMana;
    private int originalAmount;
    [SerializeField]
    private int manaPerDelivery;
    [SerializeField]
    private int manaIncreasePerLevelUp;
    [SerializeField]
    private Text manaText;
    private Vector3 oldTextPosition;
    private float timer = 0;

    public override void OnBuildingBuilt(ParentBuilding building) {
        base.OnBuildingBuilt(building);
        SetManaText(manaPerDelivery);
        oldTextPosition = manaText.rectTransform.localPosition;
    }

    public override bool LevelUp() {
        manaPerDelivery += manaIncreasePerLevelUp;
        SetManaText(manaPerDelivery);
        return base.LevelUp();
    }

    private void SetManaText(int manaCount) {
        manaText.text = "<color=blue> +" + manaCount + " </color>";
    }

    private void PlayAnimation() {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation() {
        manaText.rectTransform.localPosition = oldTextPosition;
        manaText.rectTransform.localPosition = new Vector3(manaText.rectTransform.localPosition.x, manaText.rectTransform.localPosition.y + Random.Range(1, 20), manaText.rectTransform.localPosition.z);
        manaText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        manaText.gameObject.SetActive(false);
    }

    private void Start() {
        manaPerDelivery = originalAmount;
    }

    private void OnEnable() {
        originalAmount = manaPerDelivery;
    }

    private void Update() {
        if (timer > delayBetweenMana) {
            timer = 0;
            Manager.Instance.AddMana(manaPerDelivery);
            PlayAnimation();
        }
        timer += Time.deltaTime;
    }

}
