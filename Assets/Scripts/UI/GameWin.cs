
public class GameWin : Menu {

    public void BackToMenu() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        EventManager.Instance.GameReset();
        Manager.Instance.BeforeGameScreen.SetActive(true);
        Manager.Instance.MainMenu.Show();
    }

}
