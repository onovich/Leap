using System;
using UnityEngine;

namespace Leap {

    public class RoleEntity : MonoBehaviour {

        // Base Info
        public int entityID;
        public int typeID;
        public AllyStatus allyStatus;
        public AIType aiType;

        // Attr
        public float moveSpeed;

        // FSM
        public RoleFSMComponent fsmCom;

        // Input
        public RoleInputComponent inputCom;

        // Render
        [SerializeField] public Transform body;
        [SerializeField] SpriteRenderer spr;

        // Physics
        [SerializeField] Rigidbody2D rb;

        public void Ctor() {
            fsmCom = new RoleFSMComponent();
            inputCom = new RoleInputComponent();
        }

        // Pos
        public void Pos_SetPos(Vector2 pos) {
            transform.position = pos;
        }

        public Vector2 Pos_GetPos() {
            return transform.position;
        }

        public Vector2Int Pos_GetPosInt() {
            return transform.position.RoundToVector3Int().ToVector2Int();
        }

        public Vector2 Pos_GetVolecity() {
            return rb.velocity;
        }

        // Attr
        public float Attr_GetMoveSpeed() {
            return moveSpeed;
        }

        // Move
        public void Move_ApplyMove(float dt) {
            Move_Apply(inputCom.moveAxis.normalized, Attr_GetMoveSpeed(), dt);
        }

        public void Move_MoveToTarget(Vector2 targetPos, float constrainRange, float dt) {
            float moveSpeed = Attr_GetMoveSpeed();
            Vector2 dir = targetPos - (Vector2)transform.position;
            if (dir.sqrMagnitude + float.Epsilon < constrainRange * constrainRange) {
                Move_Stop();
                return;
            }
            Move_Apply(dir, moveSpeed, dt);
        }

        public Vector2 Move_GetVelocity() {
            return rb.velocity;
        }

        public void Move_ByDir(Vector2 dir, float dt) {
            Move_Apply(dir, Attr_GetMoveSpeed(), dt);
        }

        public void Move_Stop() {
            Move_Apply(Vector2.zero, 0, 0);
        }

        void Move_Apply(Vector2 dir, float moveSpeed, float fixdt) {
            rb.velocity = dir.normalized * moveSpeed;
        }

        // FSM
        public RoleFSMStatus FSM_GetStatus() {
            return fsmCom.status;
        }

        public RoleFSMComponent FSM_GetComponent() {
            return fsmCom;
        }

        public void FSM_EnterIdle() {
            fsmCom.EnterIdle();
        }

        public void FSM_EnterDead() {
            fsmCom.EnterDead();
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