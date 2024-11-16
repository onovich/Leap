using TriInspector;
using UnityEngine;

namespace Leap.Modifier {

    public class SpikeEditorEntity : MonoBehaviour {

        [SerializeField] public SpikeTM spikeTM;
        public EntityType type => EntityType.Spike;
        public int index;


        [OnValueChanged(nameof(AdjustMeshSize))]
        [SerializeField] public Vector2 meshSize;

        [OnValueChanged(nameof(AdjustMeshSize))]
        [SerializeField] Vector2 meshOffset;

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

        public Vector2 GetMeshSize() {
            return meshSize;
        }

        public Vector2 GetMeshOffset() {
            return meshOffset;
        }

        public void AdjustMeshSize() {
            var spr = GetComponentInChildren<SpriteRenderer>();
            spr.size = meshSize;
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

        void OnDrawGizmos() {
            Gizmos.color = Color.red;

            var sizeX = meshSize.x;
            var sizeY = meshSize.y;
            var colSize = new Vector2(sizeX, sizeY);

            var pos = transform.position.ToVector2() + meshOffset;
            Gizmos.DrawWireCube(pos, colSize);
        }

    }

}