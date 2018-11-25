using UnityEngine;

public class Options : Menu {

    [SerializeField]
    private GameObject creditScreen;

    public void Back() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        Hide();
        if (Manager.Instance.IsGameRunning) {
            Sound.Instance.SaveSoundVolumeIngame();
            return;
        }
        Manager.Instance.MainMenu.Show();
    }

    public void ShowCredits() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        creditScreen.SetActive(true);
    }

    public void HideCredits() {
        Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
        creditScreen.SetActive(false);
    }

}
