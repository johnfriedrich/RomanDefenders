using Sound;
using UnityEngine;

namespace UI.MainMenu {
    public class MainMenu : Menu {

        public void StartGame() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            Manager.Manager.Instance.CharacterEditor.Show();
        }

        public void ShowOptions() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            Manager.Manager.Instance.Options.Show();
        }

        public void ShowCharacterEditor() {
            Manager.Manager.Instance.CharacterEditor.Show();
        }

        public void QuitGame() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Application.Quit();
        }

    }
}
