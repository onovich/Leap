using System;
using UnityEngine;

namespace Leap {

    public class BlockEntity : MonoBehaviour {

        // Base Info
        public int entityIndex;
        public int typeID;

        // Render
        [SerializeField] public Transform body;
        [SerializeField] SpriteRenderer spr;
        [SerializeField] BoxCollider2D boxCollider;

        // Pos
        public Vector2 Pos => transform.position;
        public Vector2Int PosInt => Pos_GetPosInt();

        public void Ctor() {
        }

        // Pos
        public void Pos_SetPos(Vector2 pos) {
            transform.position = pos;
        }

        Vector2Int Pos_GetPosInt() {
            return transform.position.RoundToVector3Int().ToVector2Int();
        }

        // Body
        public void Body_SetOffset(Vector2 offset) {
            body.localPosition = offset;
        }

        // Size
        public void Size_SetSize(Vector2 size) {
            spr.size = size;
            boxCollider.size = size;
        }

        // Mesh
        public void Mesh_Set(Sprite sp) {
            this.spr.sprite = sp;
        }

        // Rename
        public void Rename() {
            this.name = $"Block - {typeID} - {entityIndex}";
        }

        public void TearDown() {
            Destroy(gameObject);
        }

    }

}