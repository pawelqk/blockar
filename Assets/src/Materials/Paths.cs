using System.Collections.Generic;

namespace Materials
{
    public static class Paths
    {
        public static readonly List<string> MATERIAL_PATHS = new List<string>()
        {
            "Materials/basic",
            "Materials/brick",
            "Materials/wood",
            "Materials/stars"
        };

        public const string EDGES_MATERIAL_PATH = "Materials/edges";
        public const string INVISIBLE_MATERIAL_PATH = "Materials/invisible";
    }
}
