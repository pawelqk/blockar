using System.Collections.Generic;
using UnityEngine;
using VirtualObjects;
using System;

namespace Materials
{
    enum MaterialsField
    {
        Edges = 0,
        Main
    }

    public class MaterialManager
    {
        private readonly GameObject gameObjectToInstantiate;
        private readonly Dictionary<string, Material> materials;
        private readonly Material edgesMaterial;
        private readonly Material invisibleMaterial;
        private Material selectedMaterial;
        private bool isEdgesOn = true;
        
        public MaterialManager(GameObject gameObjectToInstantiate)
        {
            this.gameObjectToInstantiate = gameObjectToInstantiate;
            this.materials = Loader.LoadMaterials(Paths.MATERIAL_PATHS);
            this.selectedMaterial = null;
            this.edgesMaterial = Loader.LoadMaterial(Paths.EDGES_MATERIAL_PATH);
            this.invisibleMaterial = Loader.LoadMaterial(Paths.INVISIBLE_MATERIAL_PATH);
        }

        public void SetEdgesVisibility(Dictionary<int, GameObject> gameObjects)
        {
            isEdgesOn = !isEdgesOn;
            foreach (var gameObject in gameObjects.Values)
                SetGameObjectEdges(gameObject);
            SetGameObjectEdges(gameObjectToInstantiate);
        }

        private void SetGameObjectEdges(GameObject gameObject)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var materials = meshRenderer.materials;
            if (isEdgesOn)
                materials[(int)MaterialsField.Edges] = edgesMaterial;
            else
                materials[(int)MaterialsField.Edges] = invisibleMaterial;
            meshRenderer.materials = materials;
        }

        public void ChangeSelectedMaterial(Material material)
        {
            if (material is null)
                return;
            selectedMaterial = material;
            ChangeGameObjectMaterial(gameObjectToInstantiate);
        }

        public void SetGameObjectMaterial(GameObject gameObject, Material material)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var materials = meshRenderer.materials;
            materials[(int)MaterialsField.Main] = material;
            meshRenderer.materials = materials;
        }

        public void ChangeGameObjectMaterial(GameObject gameObject)
        {
            if (gameObject is null || selectedMaterial is null)
                return;
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            var materials = meshRenderer.materials;
            materials[(int)MaterialsField.Main] = selectedMaterial;
            meshRenderer.materials = materials;
        }

        public Dictionary<string, Material> GetMaterials()
        {
            return materials;
        }
    }
}
