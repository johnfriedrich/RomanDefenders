using System;

namespace Sound {
    [Serializable]
    public class SoundHolder {

        public SoundEnum SoundName;
        public float SoundVolumeMultiplier = 1;
        public float SoundDelay;

        public SoundHolder(SoundEnum soundName, float soundVolumeMultiplier, float soundDelay) {
            SoundName = soundName;
            SoundVolumeMultiplier = soundVolumeMultiplier;
            SoundDelay = soundDelay;
        }

    }
}
