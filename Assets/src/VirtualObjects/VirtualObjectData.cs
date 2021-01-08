using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

namespace VirtualObjects
{
    public class VirtualObjectData
    {
        private string guid;
        private GameObject gameObject = null;
        private Vector3 parentRelPosition;
        private Quaternion parentRelRotation;
        private string[] materials = null;
        private ARAnchor anchor = null;
        private Transform anchorTransform = null;
        private string cloudAnchorId = null;
        private VirtualObjectData parentingObject = null;

        public string Guid { get => guid; set => guid = value; }
        public GameObject GameObject
        {
            get => gameObject;
            set
            {
                gameObject = value;
                parentRelPosition = gameObject.transform.localPosition;
                parentRelRotation = gameObject.transform.localRotation;
                SetMaterials();
            }
        }
        public Vector3 ParentRelPosition { get => parentRelPosition; set => parentRelPosition = value; }
        public Quaternion ParentRelRotation { get => parentRelRotation; set => parentRelRotation = value; }
        public string[] Materials
        {
            get
            {
                SetMaterials();
                return materials;
            }
            set => materials = value;
        }

        private void SetMaterials()
        {
            if (gameObject is null)
                return;

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();
            materials = new string[meshRenderer.materials.Length];
            for (var i = 0u; i < meshRenderer.materials.Length; i++)
                materials[i] = meshRenderer.materials[i].name.Split(' ')[0];
        }

        public ARAnchor Anchor
        {
            get => anchor;
            set
            {
                anchor = value;
                anchorTransform = anchor.transform;
            }
        }
        public Transform AnchorTransform { get => anchorTransform; set => anchorTransform = value; }
        public string CloudAnchorId { get => cloudAnchorId; set => cloudAnchorId = value; }
        public VirtualObjectData ParentingObject { get => parentingObject; set => parentingObject = value; }

        public override string ToString()
        {
            return $"VirtualObjectData(guid={guid}, gameObjectId={gameObject?.GetInstanceID()}, pos={gameObject?.transform.position}, rot={gameObject?.transform.rotation}," +
                $" localPos={parentRelPosition}, localRot={parentRelRotation}, materials={PrintUtils.PrintCollection(materials)}, anchor={anchor}," +
                $" anchorTransformPos={anchorTransform?.position}, anchorTransformRot={anchorTransform?.rotation}, cloudAnchorId={cloudAnchorId}, parentingObject={parentingObject})";
        }

        public static VirtualObjectData FromDTO(VirtualObjectDataDTO dto)
        {
            return new VirtualObjectData {
                Guid = dto.guid,
                ParentRelPosition = new Vector3(dto.parentRelPosition.x, dto.parentRelPosition.y, dto.parentRelPosition.z),
                ParentRelRotation = new Quaternion(dto.parentRelRotation.x, dto.parentRelRotation.y, dto.parentRelRotation.z, dto.parentRelRotation.w),
                materials = dto.materials,
                CloudAnchorId = dto.cloudAnchorId,
            };
        }
    }
}
