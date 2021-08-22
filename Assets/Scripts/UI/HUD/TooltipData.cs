using UnityEngine;

namespace UI.HUD {
    public class TooltipData {

        public string Title { get; }
        public string Description { get; }
        public Sprite Sprite { get; }
        public float Amount { get; }
        public float CooldownAmount { get; }

        public TooltipData(string title, string description, Sprite sprite, float amount, float cooldownAmount) {
            Title = title;
            Description = description;
            Sprite = sprite;
            Amount = amount;
            CooldownAmount = cooldownAmount;
        }

    }
}
