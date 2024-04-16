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
        public int hp;
        public int hpMax;

        // Skill
        public bool hasWallJump;
        public bool hasDoubleJump;
        public bool hasDash;

        // State
        public bool isGround;
        public bool needTearDown;
        public bool isWall;

        // FSM
        public RoleFSMComponent fsmCom;

        // Input
        public RoleInputComponent inputCom;

        // Render
        [SerializeField] public Transform body;
        [SerializeField] SpriteRenderer spr;

        // VFX
        public string deadVFXName;
        public float deadVFXDuration;

        // Physics
        [SerializeField] Rigidbody2D rb;
        [SerializeField] RoleCollisionComponent bodyTrigger;
        [SerializeField] RoleCollisionComponent bodyCollider;

        // Pos
        public Vector2 Pos => Pos_GetPos();

        // Action
        public event Action<RoleEntity, Collider2D> OnBodyTriggerEnterHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionEnterHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionExitHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionStayHandle;

        public void Ctor() {
            fsmCom = new RoleFSMComponent();
            inputCom = new RoleInputComponent();
            Binding();
        }

        void Binding() {
            bodyTrigger.OnTriggerEnterHandle += (coll) => { OnBodyTriggerEnterHandle.Invoke(this, coll); };
            bodyCollider.OnCollisionEnterHandle += (coll) => { OnBodyCollisionEnterHandle.Invoke(this, coll); };
            bodyCollider.OnCollisionExitHandle += (coll) => { OnBodyCollisionExitHandle.Invoke(this, coll); };
            // bodyCollider.OnCollisionStayHandle += (coll) => { OnBodyCollisionStayHandle.Invoke(this, coll); };
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

        public void Attr_GetHurt() {
            hp -= 1;
        }

        // Move
        public void Move_ApplyMove(float dt) {
            Move_Apply(inputCom.moveAxis.x, Attr_GetMoveSpeed(), dt);
        }

        public void Move_Stop() {
            Move_Apply(0, 0, 0);
        }

        void Move_Apply(float xAxis, float moveSpeed, float fixdt) {
            var velo = rb.velocity;
            velo.x = xAxis * moveSpeed;
            rb.velocity = velo;
        }

        public void Move_EnterGround() {
            isGround = true;
        }

        public void Move_LeaveGround() {
            isGround = false;
        }

        public void Move_EnterWall() {
            isWall = true;
        }

        public void Move_LeaveWall() {
            isWall = false;
        }

        public void Move_Jump() {
            if (!isGround) {
                return;
            }
            if (inputCom.jumpAxis <= 0) {
                return;
            }
            var velo = rb.velocity;
            velo.y = jumpForce;
            rb.velocity = velo;
            Move_LeaveGround();
        }

        public void Move_WallJump() {
            if (!isWall) {
                return;
            }
            if (inputCom.jumpAxis <= 0) {
                return;
            }
            var velo = rb.velocity;
            velo.y = jumpForce;
            rb.velocity = velo;
            Move_LeaveWall();
        }

        public void Move_Falling(float dt) {
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
            bodyTrigger.TearDown();
            Destroy(this.gameObject);
        }

    }

}