using System.Collections.Generic;

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
        bool containsCharacter = false;
        for (int i = 0; i < parentObjects.Count; i++) {
            if (parentObjects[i] is Character) {
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
