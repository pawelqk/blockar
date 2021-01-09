using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    public static class Loader
    {
        public static Dictionary<string, AudioClip> LoadAudioClips(List<string> paths)
        {
            var audioClips = new Dictionary<string, AudioClip>();
            foreach (string path in paths)
            {
                var audioClip = LoadAudioClip(path);
                if (audioClip)
                    audioClips.Add(audioClip.name, audioClip);
            }
            return audioClips;
        }

        public static AudioClip LoadAudioClip(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                Debug.LogError("Path: " + path + " is invalid.");
                return null;
            }

            var audioClip = Resources.Load<AudioClip>(path);
            if (audioClip is null)
            {
                Debug.LogError("Couldn't load " + path + " material.");
            }
            return audioClip;
        }
    }
}
