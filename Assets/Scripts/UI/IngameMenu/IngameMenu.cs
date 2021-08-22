using Manager;
using Sound;

namespace UI.IngameMenu {
    public class IngameMenu : Menu {

        public void PlayClickSound() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        }

        public void ShowOptions() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            Manager.Manager.Instance.Options.Show();
        }

        public void ShowHelp() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            Manager.Manager.Instance.Help.Show();
        }

        public void QuitRunningGame() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            EventManager.Instance.QuitGame();
            EventManager.Instance.GameReset();
            Manager.Manager.Instance.BeforeGameScreen.SetActive(true);
            Manager.Manager.Instance.MainMenu.Show();
        }

    }
}
