using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Chess_FirstStep
{
    using Android.Media;

    public class SoundManager
    {
        private SoundPool soundPool;
        private Dictionary<int, int> soundMap;

        public SoundManager()
        {
            // Initialize SoundPool with a maximum of 10 simultaneous streams
            soundPool = new SoundPool(10, Stream.Music, 0);
            soundMap = new Dictionary<int, int>();
        }

        public void LoadSound(Context context, int soundResourceId, int soundId)
        {
            // Load a sound into the SoundPool and map it to a specific soundId
            int loadedSoundId = soundPool.Load(context, soundResourceId, 1);
            soundMap[soundId] = loadedSoundId;
        }

        public void PlaySound(int soundId, float leftVolume = 1.0f, float rightVolume = 1.0f, int priority = 1, int loop = 0, float rate = 1.0f)
        {
            if (soundMap.ContainsKey(soundId))
            {
                int loadedSoundId = soundMap[soundId];
                soundPool.Play(loadedSoundId, leftVolume, rightVolume, priority, loop, rate);
            }
        }
    }
}