
using Manager;
using Sound;

public class GameLost : Menu {

    public void BackToMenu() {
        Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        EventManager.Instance.QuitGame();
        EventManager.Instance.GameReset();
        Manager.Manager.Instance.BeforeGameScreen.SetActive(true);
        Manager.Manager.Instance.MainMenu.Show();
    }

}
