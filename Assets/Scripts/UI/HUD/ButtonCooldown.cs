using UnityEngine;
using UnityEngine.UI;

public class ButtonCooldown : MonoBehaviour {

    [SerializeField]
    private Image coolddownImage;
    [SerializeField]
    private Text cooldownText;
    private float cooldown;
    private float originalCooldown;

    public float RemainingCooldown {
        get {
            return cooldown;
        }
    }

    public bool IsFinished() {
        return cooldown <= 0.0f;
    }

    public void SetupCooldown(Text textTolLink, Image imageToLink) {
            coolddownImage = imageToLink;
            cooldownText = textTolLink;
        if (IsFinished()) {
            Finish();
        }
    }

    public void SetCooldown(float desiredCooldownTime) {
        originalCooldown = desiredCooldownTime;
        cooldown = originalCooldown;
        cooldownText.text = Mathf.RoundToInt(cooldown).ToString();
        coolddownImage.fillAmount = 1;
    }

    public void Clear() {
        coolddownImage = null;
        cooldownText = null;
    }

    public void Finish() {
        if (cooldownText != null) {
            cooldownText.text = string.Empty;
            coolddownImage.fillAmount = 0;
        }
        cooldown = 0;
    }

    private void Update() {
        if (cooldown > 0.0f) {
            cooldown -= Time.deltaTime;
            if (cooldownText != null) {
                cooldownText.text = Mathf.RoundToInt(cooldown).ToString();
                coolddownImage.fillAmount = cooldown / originalCooldown;
            }
            
        }
        if (cooldown <= 0) {
            Finish();
        }
    }

    private void Start() {
        EventManager.Instance.OnStartGamePostEvent += Finish;
    }

}
