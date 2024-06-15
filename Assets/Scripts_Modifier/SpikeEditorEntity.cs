using UnityEngine;

namespace Leap.Modifier {

    public class SpikeEditorEntity : MonoBehaviour {

        [SerializeField] public SpikeTM spikeTM;
        public EntityType type => EntityType.Spike;
        public int index;

        public void Rename() {
            this.gameObject.name = $"Spike - {spikeTM.typeID} - {index}";
        }

        public Vector2Int GetPosInt() {
            var posInt = this.transform.position.RoundToVector2Int();
            this.transform.position = posInt.ToVector3Int();
            return posInt;
        }

        public Vector2Int GetSizeInt() {
            var size = GetComponent<SpriteRenderer>().size;
            var sizeInt = size.RoundToVector2Int();
            return sizeInt;
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