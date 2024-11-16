using TriInspector;
using UnityEngine;

namespace Leap.Modifier {

    public class BlockEditorEntity : MonoBehaviour {

        [SerializeField] public BlockTM blockTM;
        public EntityType type => EntityType.Block;
        public int index;

        Vector2 meshOffset;

        [OnValueChanged(nameof(AdjustMeshSize))]
        [SerializeField] Vector2Int meshSize;

        public void Rename() {
            this.gameObject.name = $"Block - {blockTM.typeID} - {index}";
        }

        void SetMeshOffset() {
            var trans = GetComponentInChildren<SpriteRenderer>().transform;
            trans.localPosition = meshOffset;
        }

        public void AdjustMeshSize() {
            var spr = GetComponentInChildren<SpriteRenderer>();
            var trans = GetComponentInChildren<SpriteRenderer>().transform;
            spr.size = meshSize;

            var halfSize = meshSize / 2;
            var center = trans.localPosition.ToVector2();
            var ld = center - halfSize;

            // var meshOffsetX = ld.x % 2 == 0 ? 0f : 0.5f;
            // var meshOffsetY = ld.y % 2 == 0 ? 0f : 0.5f;
            // meshOffset = new Vector2(meshOffsetX, meshOffsetY);
            var meshOffsetX = meshSize.x % 2 == 0 ? 0.5f : 0f;
            var meshOffsetY = meshSize.y % 2 == 0 ? 0.5f : 0f;
            meshOffset = new Vector2(meshOffsetX, meshOffsetY);
            SetMeshOffset();
        }

        public Vector2Int GetPosInt() {
            var posInt = this.transform.position.RoundToVector2Int();
            this.transform.position = posInt.ToVector3Int();
            return posInt;
        }

        public Vector2Int GetMeshSize() {
            return meshSize;
        }

        public Vector2 GetMeshOffset() {
            return meshOffset;
        }

        public void OnDrawGhost(Vector3 offset) {
            if (blockTM == null) return;
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            var pos = transform.position + offset;
            Gizmos.DrawCube(offset, Vector3.one);
        }
    }
}