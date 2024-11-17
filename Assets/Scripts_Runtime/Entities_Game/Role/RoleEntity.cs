using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
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
        public float jumpForceY;
        public float wallJumpForceYMax;
        public float wallJumpForceYCurrent;
        public float wallJumpForceXMax;
        public float wallJumpForceXCurrent;
        public float wallJumpDuration;
        public float wallingDuration;
        public float wallJumpAccelerationX;
        public float wallJumpAccelerationY;

        public float dashSpeed;
        public float dashAcceleration;
        public float dashDuration;
        public float dashForceMax;
        public float dashForceCurrent;

        public float landDuration;
        public float g;
        public float fallingSpeedMax;
        public Vector2 Velocity => rb.velocity;
        public int hp;
        public int hpMax;

        // Skill
        public bool hasWallJump;
        public bool hasDoubleJump;
        public bool hasDash;

        // Gizmos
        public Color gizmosTextColor;
        public Vector3 gizmosTextOffset;
        public string gizmosText;

        public Color gizmosColliderColor;
        public Vector3 gizmosColliderOffset;
        public Vector2 gizmosColliderSize;

        // Physics
        public bool physics_hitGround;
        public bool physics_hitSpike;
        public bool physics_hitWall;
        public Vector2 physics_hitWallDir;
        public float physics_wallFriction;
        public bool physics_hitCeiling;
        public Vector2 physics_hitCeilingDir;

        // State
        public bool needTearDown;
        public Vector2 lastFaceDir;
        public Vector2 lastVelocityNormalized;

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
        [SerializeField] RoleCollisionComponent headCollider;
        [SerializeField] RoleCollisionComponent footCollider;

        Vector2 bodyColSize;
        public Vector2 BodyColSize => bodyColSize;

        Vector2 headColSize;
        public Vector2 HeadColSize => headColSize;

        // Pos
        public Vector2 Pos => Pos_GetPos();

        // Action
        public event Action<RoleEntity, Collider2D> OnBodyTriggerEnterHandle;
        public event Action<RoleEntity, Collision2D> OnBodyCollisionStayHandle;
        public event Action<RoleEntity, Collision2D> OnHeadCollisionEnterHandle;
        public event Action<RoleEntity, Collision2D> OnFootCollisionEnterHandle;

        public void Ctor() {
            fsmCom = new RoleFSMComponent();
            inputCom = new RoleInputComponent();
            Binding();
            lastFaceDir = Vector2.right;
        }

        void Binding() {
            bodyTrigger.OnTriggerEnterHandle += (coll) => {
                OnBodyTriggerEnterHandle.Invoke(this, coll);
            };
            bodyCollider.OnCollisionStayHandle += (coll) => {
                OnBodyCollisionStayHandle.Invoke(this, coll);
            };
            headCollider.OnCollisionEnterHandle += (coll) => {
                OnHeadCollisionEnterHandle.Invoke(this, coll);
            };
            footCollider.OnCollisionEnterHandle += (coll) => {
                OnFootCollisionEnterHandle.Invoke(this, coll);
            };
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

        public void Attr_DeadlyHurt() {
            hp = 0;
        }

        // Velocity
        public void Velocity_Set(Vector2 v) {
            rb.velocity = v;
            lastVelocityNormalized = v.normalized;
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
            Velocity_Set(velo);
            RecordLastFaceDir(rb.velocity);
        }

        public void Color_SetColor(Color color) {
            spr.color = color;
        }

        public bool Move_Jump() {
            var velo = rb.velocity;
            velo.y = jumpForceY;
            Velocity_Set(velo);
            return true;
        }

        public void Move_Dash(Vector2 dir) {
            var velo = dir.normalized * dashSpeed * dashForceMax;
            Velocity_Set(velo);
            dashForceCurrent = dashForceMax;
            RecordLastFaceDir(rb.velocity);
        }

        public void Move_DashForceTick(Vector2 dir, float dt) {
            dashForceCurrent += dashAcceleration * dashForceCurrent;
            var velo = dir.normalized * dashSpeed * dashForceCurrent;
            Velocity_Set(velo);
        }

        public void Move_WallJump(Vector2 dir) {
            var velo = rb.velocity;
            velo.y = wallJumpForceYMax;
            velo.x = dir.x * wallJumpForceXMax;
            Velocity_Set(velo);

            wallJumpForceYCurrent = wallJumpForceYMax;
            wallJumpForceXCurrent = wallJumpForceXMax;

            RecordLastFaceDir(rb.velocity);
        }

        public void Move_WallJumpForceTick(Vector2 dir, float dt) {
            wallJumpForceYCurrent += wallJumpAccelerationY * wallJumpForceYCurrent;
            wallJumpForceXCurrent += wallJumpAccelerationX * wallJumpForceXCurrent;
            var velo = rb.velocity;
            velo.y = wallJumpForceYCurrent;
            velo.x = dir.x * wallJumpForceXCurrent;
            Velocity_Set(velo);
        }

        public void Move_Falling(float wallFriction, float dt) {
            var velo = rb.velocity;
            velo.y -= g * dt;

            velo.y *= 1 - wallFriction;

            if (velo.y < -fallingSpeedMax) {
                velo.y = -fallingSpeedMax;
            }

            Velocity_Set(velo);
        }

        // FSM
        public RoleFSMStatus FSM_GetStatus() {
            return fsmCom.status;
        }

        public RoleFSMComponent FSM_GetComponent() {
            return fsmCom;
        }

        // Mesh
        public void Mesh_Set(Sprite sp) {
            this.spr.sprite = sp;
        }

        public void TearDown() {
            bodyTrigger.TearDown();
            Destroy(this.gameObject);
        }

        // Record
        void RecordLastFaceDir(Vector2 velocity) {
            if (velocity.x >= 0) {
                lastFaceDir = Vector2.right;
            } else {
                lastFaceDir = Vector2.left;
            }
        }

        // Physics
        public void Size_SetBodyCollSize(Vector2 size) {
            this.bodyColSize = size;
        }

        public void Size_SetHeadCollSize(Vector2 size) {
            this.headColSize = size;
        }

        public void Physics_ResetHitWall() {
            physics_hitWall = false;
            physics_wallFriction = 0f;
            physics_hitWallDir = Vector2.zero;
        }

        public void Physics_ResetLandGround() {
            physics_hitGround = false;
        }

        public void Physics_ResetHitSpike() {
            physics_hitSpike = false;
        }

        // Gizmos
#if UNITY_EDITOR
        public void OnDrawDebugGizmos() {
            // 设置文字的颜色
            GUIStyle style = new GUIStyle();
            style.normal.textColor = gizmosTextColor;

            // 在物体位置上方绘制文字
            Handles.Label(transform.position + gizmosTextOffset, gizmosText, style);

            // 绘制碰撞检测盒
            // Gizmos.color = gizmosColliderColor;
            // Gizmos.DrawWireCube(transform.position + gizmosColliderOffset, gizmosColliderSize);

            // 绘制碰撞方向
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(physics_hitWallDir.x, physics_hitWallDir.y, 0));
        }
#endif

    }

}