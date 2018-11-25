using UnityEngine;

public class MainMenu : Menu {

    public void StartGame() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        Manager.Instance.CharacterEditor.Show();
    }

    public void ShowOptions() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        Manager.Instance.Options.Show();
    }

    public void ShowCharacterEditor() {
        Manager.Instance.CharacterEditor.Show();
    }

    public void QuitGame() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Application.Quit();
    }

}
