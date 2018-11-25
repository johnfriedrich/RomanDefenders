using UnityEngine;
using UnityEngine.UI;

public class BoundTooltipItem : MonoBehaviour {

    public Text TooltipText;
    public Vector3 ToolTipOffset;

    private static BoundTooltipItem instance;

    [SerializeField]
    private Canvas canvas;

    public static BoundTooltipItem Instance {
        get {
            if (instance == null)
                instance = FindObjectOfType<BoundTooltipItem>();
            return instance;
        }
    }

    public bool IsActive {
        get {
            return gameObject.activeSelf;
        }
    }

    public void ShowTooltip(string text, Vector3 currentPosition, Vector3 offset) {
        if (TooltipText.text != text)
            TooltipText.text = text;

        //This corrects the tooltip offset for resolutions bigger than FullHD. Ultra HD for example
        Vector3 finalPosistion = currentPosition + (offset + ToolTipOffset) * canvas.scaleFactor;

        transform.position = finalPosistion;
        gameObject.SetActive(true);
    }

    public void HideTooltip() {
        gameObject.SetActive(false);
    }

    private void Awake() {
        instance = this;
        HideTooltip();
    }

}