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
        public float jumpForce;
        public float g;
        public float fallingSpeedMax;
        public Vector2 Velocity => rb.velocity;

        // State
        public bool isGround;

        // FSM
        public RoleFSMComponent fsmCom;

        // Input
        public RoleInputComponent inputCom;

        // Render
        [SerializeField] public Transform body;
        [SerializeField] SpriteRenderer spr;

        // Physics
        [SerializeField] Rigidbody2D rb;
        [SerializeField] RoleCollisionComponent bodyCollider;
        [SerializeField] RoleCollisionComponent bodyTrigger;
        [SerializeField] RoleCollisionComponent footTrigger;

        // Pos
        public Vector2 Pos => Pos_GetPos();

        // Action
        public event Action<RoleEntity, Collider2D> OnFootTriggerEnterHandle;
        public event Action<RoleEntity, Collider2D> OnFootTriggerStayHandle;
        public event Action<RoleEntity, Collider2D> OnFootTriggerExitHandle;

        public event Action<RoleEntity, Collision2D> OnBodyCollisionEnterHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionStayHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionExitHandle;

        public event Action<RoleEntity, Collider2D> OnBodyTriggerEnterHandle;

        public void Ctor() {
            fsmCom = new RoleFSMComponent();
            inputCom = new RoleInputComponent();
            Binding();
        }

        void Binding() {
            footTrigger.OnTriggerEnterHandle += (coll) => { OnFootTriggerEnterHandle.Invoke(this, coll); };
            footTrigger.OnTriggerStayHandle += (coll) => { OnFootTriggerStayHandle.Invoke(this, coll); };
            footTrigger.OnTriggerExitHandle += (coll) => { OnFootTriggerExitHandle.Invoke(this, coll); };

            bodyCollider.OnCollisionEnterHandle += (coll) => { OnBodyCollisionEnterHandle.Invoke(this, coll); };
            bodyCollider.OnCollisionStayHandle += (coll) => { OnBodyCollisionStayHandle.Invoke(this, coll); };
            bodyCollider.OnCollisionExitHandle += (coll) => { OnBodyCollisionExitHandle.Invoke(this, coll); };

            bodyTrigger.OnTriggerEnterHandle += (coll) => { OnBodyTriggerEnterHandle.Invoke(this, coll); };
        }

        // Pos
        public void Pos_SetPos(Vector2 pos) {
            transform.position = pos;
        }

        Vector2 Pos_GetPos() {
            return transform.position;
        }

        // Attr
        public float Attr_GetMoveSpeed() {
            return moveSpeed;
        }

        // Move
        public void Move_ApplyMove(float dt) {
            Move_Apply(inputCom.moveAxis.normalized, Attr_GetMoveSpeed(), dt);
        }

        public void Move_Stop() {
            Move_Apply(Vector2.zero, 0, 0);
        }

        void Move_Apply(Vector2 dir, float moveSpeed, float fixdt) {
            rb.velocity = dir.normalized * moveSpeed;
        }

        public void Move_EnterGround() {
            isGround = true;
        }

        public void Move_LeaveGround() {
            isGround = false;
        }

        public void Move_Jump() {
            if (!isGround) {
                return;
            }
            var velo = rb.velocity;
            velo.y = jumpForce;
            rb.velocity = velo;
            Move_LeaveGround();
        }

        public void Move_Falling(float dt) {
            if (isGround) {
                return;
            }
            var velo = rb.velocity;
            velo.y -= g * dt;
            velo.y = Mathf.Max(velo.y, -fallingSpeedMax);
            rb.velocity = velo;
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
            footTrigger.TearDown();
            bodyCollider.TearDown();
            bodyTrigger.TearDown();
            Destroy(this.gameObject);
        }

    }

}