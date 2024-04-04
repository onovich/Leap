using System;
using UnityEngine;

namespace Leap {

    public class BlockEntity : MonoBehaviour {

        // Base Info
        public int entityID;
        public int typeID;

        // Render
        [SerializeField] public Transform body;
        [SerializeField] SpriteRenderer spr;

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

        // Mesh
        public void Mesh_Set(Sprite sp) {
            this.spr.sprite = sp;
        }

        public void TearDown() {
            Destroy(gameObject);
        }

    }

}