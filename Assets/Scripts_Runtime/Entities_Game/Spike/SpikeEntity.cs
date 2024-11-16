using System;
using UnityEngine;

namespace Leap {

    public class SpikeEntity : MonoBehaviour {

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

        // Size
        public void Size_SetSize(Vector2 size) {
            spr.size = size;
            // size.y *= 0.5f;
            boxCollider.size = size;
        }

        // Body
        public void Body_SetOffset(Vector2 offset) {
            body.localPosition = offset;
        }

        // Mesh
        public void Mesh_Set(Sprite sp) {
            this.spr.sprite = sp;
        }

        // Rename
        public void Rename() {
            this.name = $"Spike - {typeID} - {entityIndex}";
        }

        public void TearDown() {
            Destroy(gameObject);
        }

    }

}