using TriInspector;
using UnityEngine;

namespace Leap.Modifier {

    public class SpikeEditorEntity : MonoBehaviour {

        [SerializeField] public SpikeTM spikeTM;
        public EntityType type => EntityType.Spike;
        public int index;

        Vector2 meshOffset;

        [OnValueChanged(nameof(AdjustMeshSize))]
        [SerializeField] Vector2Int meshSize;

        public void Rename() {
            this.gameObject.name = $"Spike - {spikeTM.typeID} - {index}";
        }

        public Vector2Int GetPosInt() {
            var posInt = this.transform.position.RoundToVector2Int();
            this.transform.position = posInt.ToVector3Int();
            return posInt;
        }

        void SetMeshOffset() {
            var trans = GetComponentInChildren<SpriteRenderer>().transform;
            trans.localPosition = meshOffset;
        }

        public Vector2Int GetMeshSize() {
            return meshSize;
        }

        public Vector2 GetMeshOffset() {
            return meshOffset;
        }

        public void AdjustMeshSize() {
            var spr = GetComponentInChildren<SpriteRenderer>();
            spr.size = meshSize;

            var meshOffsetX = meshSize.x % 2 == 0 ? 0.5f : 0f;
            var meshOffsetY = meshSize.y % 2 == 0 ? 0.5f : 0f;
            meshOffset = new Vector2(meshOffsetX, meshOffsetY);
            SetMeshOffset();
        }

        public int GetRotationZInt() {
            var rotation = this.transform.rotation;
            var rotationZInt = Mathf.RoundToInt(rotation.z);
            rotation.z = rotationZInt;
            this.transform.rotation = rotation;
            return rotationZInt;
        }

        public void OnDrawGhost(Vector3 offset) {
            if (spikeTM == null) return;
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            var pos = transform.position + offset;
            Gizmos.DrawCube(offset, Vector3.one);
        }

    }

}