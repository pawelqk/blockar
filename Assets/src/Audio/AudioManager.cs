using System.Collections.Generic;
using UnityEngine;
using System;

namespace Audio
{
    public static class AudioManager
    {
        private static readonly Dictionary<string, AudioClip> audioClips;
        private static readonly AudioSource audioSrc;

        static AudioManager()
        {
            audioSrc = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            audioClips = Loader.LoadAudioClips(AudioClips.AUDIO_PATHS);
        }

        public static void PlayAudioClip(string audioClipName)
        {
            var clipExists = audioClips.TryGetValue(audioClipName, out AudioClip audioClip);
            if (clipExists)
                audioSrc.PlayOneShot(audioClip); 
            else
                Debug.LogError("Couldn't find " + audioClipName + " audio clip.");
        }
    }
}
