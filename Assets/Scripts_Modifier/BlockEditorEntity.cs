using UnityEngine;

namespace Leap.Modifier {

    public class BlockEditorEntity : MonoBehaviour {

        [SerializeField] public BlockTM blockTM;
        public int index;

        public void Rename() {
            this.gameObject.name = $"Block - {blockTM.typeID} - {index}";
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

    }

}