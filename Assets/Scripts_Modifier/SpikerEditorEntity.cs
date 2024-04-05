using UnityEngine;

namespace Leap.Modifier {

    public class SpikerEditorEntity : MonoBehaviour {

        [SerializeField] public SpikeTM spikeTM;
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
            var size = transform.localScale;
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

    }

}