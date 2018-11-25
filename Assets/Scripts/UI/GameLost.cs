
public class GameLost : Menu {

    public void BackToMenu() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        EventManager.Instance.QuitGame();
        EventManager.Instance.GameReset();
        Manager.Instance.BeforeGameScreen.SetActive(true);
        Manager.Instance.MainMenu.Show();
    }

}
