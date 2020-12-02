using System;
using System.Collections.Generic;
using UnityEngine;
using VirtualObjects;

namespace Materials
{
    public static class Loader
    {
        public static Dictionary<string, Material> LoadMaterials(List<string> paths)
        {
            var materials = new Dictionary<string, Material>();
            foreach (string path in paths)
            {
                var material = LoadMaterial(path);
                if (material)
                    materials.Add(material.name, material);
            }
            return materials;
        }

        public static Material LoadMaterial(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                Debug.LogError("Path: " + path + " is invalid.");
                return null;
            }

            var material = Resources.Load<Material>(path);
            if (material is null)
            {
                Debug.LogError("Couldn't load " + path + " material.");
            }
            return material;
        }
    }
}
