using Sound;
using UnityEngine;

namespace UI.Options {
    public class Options : Menu {

        [SerializeField]
        private GameObject creditScreen;

        public void Back() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            if (Manager.Manager.Instance.IsGameRunning) {
                Sound.Sound.Instance.SaveSoundVolumeIngame();
                return;
            }
            Manager.Manager.Instance.MainMenu.Show();
        }

        public void ShowCredits() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            creditScreen.SetActive(true);
        }

        public void HideCredits() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            creditScreen.SetActive(false);
        }

    }
}
