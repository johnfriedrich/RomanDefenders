using System.Collections;
using System.Linq;
using Entities;
using Manager;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Sound {
    public class Sound : MonoBehaviour {

        public static Sound Instance { get; private set; }

        private const float DefaultSoundVolume = 0.5f;
        private const float DefaultMusicVolume = 0.75f;
        private const float DefaultAmbientVolume = 0.5f;

        [SerializeField]
        private AudioSource musicPlayer;
        [SerializeField]
        private AudioSource soundPlayer;
        [SerializeField]
        private AudioSource ambientPlayer;
        [SerializeField]
        private Slider musicVolumeSlider;
        [SerializeField]
        private Slider soundVolumeSlider;
        [SerializeField]
        private Slider ambientVolumeSlider;
        [SerializeField]
        private MusicData[] musicClips;
        [SerializeField]
        private SoundData[] soundClips;
        private float soundVolume;
        private float sinceLastBattle = 0;
        private AudioClip fightMusic;
        private AudioClip fightMusic2;

        public float SoundVolume => soundVolume;

        public void PlayMusicClip(MusicEnum searchedMusic, bool shouldLoop) {
            foreach (MusicData data in musicClips) {
                if (data.name == searchedMusic) {
                    musicPlayer.clip = data.clip;
                    musicPlayer.loop = shouldLoop;
                    musicPlayer.Play();
                }
            }
        }

        public void PlaySoundClip(SoundEnum searchEnum) {
            foreach (SoundData data in soundClips) {
                if (data.name == searchEnum) {
                    //soundPlayer.clip = data.clip;
                    soundPlayer.PlayOneShot(data.clip);
                }
            }
        }

        public void AdjustMusicVolume() {
            musicPlayer.volume = musicVolumeSlider.value;
        }

        public void AdjustSoundVolume() {
            soundVolume = soundVolumeSlider.value;
            soundPlayer.volume = soundVolume;
        }

        public void AdjustAmbientVolume() {
            ambientPlayer.volume = ambientVolumeSlider.value;
        }

        public void SaveSoundVolumeIngame() {
            //Please no hate because of this :(
            //FindByType<AudioSource>() would also return Sources on Manager which shouldn't be touched. By tag might be faster anyway
            //And yes, I am aware of mixers, but they didn't work out that great
            var entitySources = GameObject.FindGameObjectsWithTag(TagData.Entity).Select(x => x.GetComponent<AudioSource>()).ToArray();
            foreach (var t in entitySources) {
                t.volume = soundVolume;
            }
            var buildingSources = GameObject.FindGameObjectsWithTag(TagData.Building).Select(x => x.GetComponent<AudioSource>()).ToArray();
            foreach (var t in buildingSources) {
                t.volume = soundVolume;
            }
        }

        public void PlaySoundClipWithSource(SoundHolder soundHolder, AudioSource source, float duration) {
            foreach (SoundData data in soundClips) {
                if (data.name == soundHolder.SoundName) {
                    source.clip = data.clip;
                    source.PlayOneShot(source.clip, soundHolder.SoundVolumeMultiplier);
                    if (duration > 0) {
                        StartCoroutine(PlayForDuration(duration, source));
                    }
                }
            }
        }

        public void PlaySoundClipWithSource(SoundHolder soundHolder, AudioSource source, float duration, bool loop) {
            if (loop) {
                foreach (var data in soundClips) {
                    if (data.name == soundHolder.SoundName) {
                        source.clip = data.clip;
                        source.loop = loop;
                        source.Play();
                    }
                }
                return;
            }
            PlaySoundClipWithSource(soundHolder, source, duration);
        }

        private void Awake() {
            Instance = this;
            soundVolume = DefaultSoundVolume;
            FillFightSound();
        }

        private void FillFightSound() {
            foreach (var data in musicClips) {
                if (data.name == MusicEnum.Roman_Fight1) {
                    fightMusic = data.clip;

                }
            }
            foreach (var data in musicClips) {
                if (data.name == MusicEnum.Roman_Fight2) {
                    fightMusic2 = data.clip;

                }
            }
        }

        private void Start() {
            musicPlayer.volume = DefaultMusicVolume;
            soundPlayer.volume = DefaultSoundVolume;
            musicVolumeSlider.value = DefaultMusicVolume;
            soundVolumeSlider.value = DefaultSoundVolume;
            EventManager.Instance.OnStartGameEvent += IngameMusicLoop;
            EventManager.Instance.OnGameResetEvent += MenuMusicLoop;
            EventManager.Instance.OnEntityDamageEvent += HandleBattleMusicWrapper;
            EventManager.Instance.OnBuildingDamageEvent += HandleBattleMusicWrapper;
            EventManager.Instance.OnGameResetPostEvent += ResetBattleTimer;
            MenuMusicLoop();
        }

        private void IngameMusicLoop() {
            PlayMusicClip(MusicEnum.Roman_Settle2, true);
        }

        private void MenuMusicLoop() {
            PlayMusicClip(MusicEnum.Roman_Settle1, true);
        }

        private IEnumerator PlayForDuration(float duration, AudioSource source) {
            source.Play();
            source.loop = true;
            yield return new WaitForSeconds(duration);
            source.loop = false;
            source.Stop();
        }

        private void ResetBattleTimer() {
            sinceLastBattle = 0;
        }

        private void HandleBattleMusicWrapper(Entity entitiy) {
            HandleBattleMusic();
        }

        private void HandleBattleMusicWrapper(ParentBuilding building) {
            HandleBattleMusic();
        }

        private void HandleBattleMusic() {
            if (sinceLastBattle != 0) {
                return;
            }
            var rnd = Random.Range(0, 2);
            if (rnd == 1) {
                PlayMusicClip(MusicEnum.Roman_Fight1, true);
            } else {
                PlayMusicClip(MusicEnum.Roman_Fight2, true);
            }
        }

        private void Update() {
            if (musicPlayer.clip == fightMusic || musicPlayer.clip == fightMusic2) {
                sinceLastBattle += Time.deltaTime;
                if (sinceLastBattle > 20) {
                    sinceLastBattle = 0;
                    IngameMusicLoop();
                }
            }
        }

    }
}
