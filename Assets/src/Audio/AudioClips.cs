using System.Collections.Generic;

namespace Audio
{
    public static class AudioClips
    {
        public static readonly List<string> AUDIO_PATHS = new List<string>()
        {
            "Audio/crackle",
            "Audio/knock",
            "Audio/placing_error"
        };

        public static readonly string CRACKLE = "crackle";
        public static readonly string KNOCK = "knock";
        public static readonly string PLACING_ERROR = "placing_error";
    }
}
