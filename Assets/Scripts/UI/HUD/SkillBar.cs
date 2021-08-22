using System.Collections.Generic;
using Entities;
using Manager;
using Parent;

namespace UI.HUD {
    public class SkillBar : Menu {

        private void Start () {
            EventManager.Instance.OnParentObjectSelectedEvent += Activate;
            EventManager.Instance.OnDeselectEvent += Hide;
            EventManager.Instance.OnEntityDeathEvent += HideWrapper;
            Hide();
        }

        private void HideWrapper(Entity entity) {
            if (entity is Character) {
                Hide();
            }
        }

        private void Activate(List<ParentObject> parentObjects) {
            var containsCharacter = false;
            foreach (var t in parentObjects) {
                if (t is Character) {
                    containsCharacter = true;
                    break;
                }
            }
            if (containsCharacter) {
                Show();
            } else {
                Hide();
            }
        }

    }
}
