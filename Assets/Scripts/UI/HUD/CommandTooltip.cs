using UnityEngine;
using UnityEngine.UI;

namespace UI.HUD {
    public class CommandTooltip : Menu {

        [SerializeField]
        private Text title;
        [SerializeField]
        private Text description;
        [SerializeField]
        private Image sprite;
        [SerializeField]
        private Text amount;
        [SerializeField]
        private Text cooldownAmount;
        [SerializeField]
        private GameObject cooldownText;

        public void SetTooltip(TooltipData tooltipData) {
            gameObject.SetActive(true);
            cooldownText.SetActive(false);
            title.text = tooltipData.Title;
            description.text = tooltipData.Description;
            sprite.enabled = true;
            if (tooltipData.Sprite == null) {
                sprite.enabled = false;
            } else {
                sprite.sprite = tooltipData.Sprite;
                sprite.preserveAspect = true;
            }
            if (tooltipData.Amount == 0) {
                amount.text = string.Empty;
            } else {
                amount.text = tooltipData.Amount.ToString();
            }
            if (tooltipData.CooldownAmount == 0) {
                cooldownAmount.text = string.Empty;
            } else {
                cooldownText.SetActive(true);
                cooldownAmount.text = tooltipData.CooldownAmount.ToString();
            }
        }

    }
}
