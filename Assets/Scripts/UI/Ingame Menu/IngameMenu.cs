
public class IngameMenu : Menu {

    public void PlayClickSound() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
    }

    public void ShowOptions() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        Manager.Instance.Options.Show();
    }

    public void ShowHelp() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        Manager.Instance.Help.Show();
    }

    public void QuitRunningGame() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        EventManager.Instance.QuitGame();
        EventManager.Instance.GameReset();
        Manager.Instance.BeforeGameScreen.SetActive(true);
        Manager.Instance.MainMenu.Show();
    }

}
