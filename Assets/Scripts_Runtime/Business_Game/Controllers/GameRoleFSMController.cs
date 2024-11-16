using Codice.Client.BaseCommands.Config;
using UnityEngine;

namespace Leap {

    public static class GameRoleFSMController {

        public static void FixedTickFSM(GameBusinessContext ctx, RoleEntity role, float fixdt) {

            FixedTickFSM_Any(ctx, role, fixdt);

            RoleFSMStatus status = role.FSM_GetStatus();
            if (status == RoleFSMStatus.Landing) {
                FixedTickFSM_Landing(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.white, Color.red, "陆");
            } else if (status == RoleFSMStatus.Airing) {
                FixedTickFSM_Airing(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.green, Color.white, "空");
            } else if (status == RoleFSMStatus.Jumping) {
                FixedTickFSM_Jumping(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.yellow, Color.red, "跳");
            } else if (status == RoleFSMStatus.Walling) {
                FixedTickFSM_Walling(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.red, Color.white, "墙");
            } else if (status == RoleFSMStatus.WallJumping) {
                FixedTickFSM_WallJumping(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.blue, Color.white, "蹬");
            } else if (status == RoleFSMStatus.Dying) {
                FixedTickFSM_Dying(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.black, Color.white, "亡");
            } else {
                GLog.LogError($"GameRoleFSMController.FixedTickFSM: unknown status: {status}");
            }

        }

        static void SetFSMGizmos(GameBusinessContext ctx, RoleEntity role, Color roleColor, Color gizmosColor, string gizmosText) {
            if (!ctx.gameEntity.IsDebugMode) {
                return;
            }
            role.Color_SetColor(roleColor);
            role.gizmosTextColor = gizmosColor;
            role.gizmosText = gizmosText;
        }

        static void FixedTickFSM_Any(GameBusinessContext ctx, RoleEntity role, float fixdt) {
        }

        static void FixedTickFSM_Airing(GameBusinessContext ctx, RoleEntity role, float dt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.airing_isEntering) {
                fsm.airing_isEntering = false;
                role.Move_Stop();
            }

            // Fall
            bool succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, dt);

            // Hit Wall
            succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, dt);

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, dt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Jumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            var input = ctx.inputEntity;
            if (fsm.jumping_isEntering) {
                fsm.jumping_isEntering = false;
                input.ResetJumpAxisTemp();
                GameRoleDomain.ApplyJump(ctx, role, fixdt);
                return;
            }

            // Fall
            bool succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                // Debug.Log("Jumping -> Landing");
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Hit Wall
            succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                // Debug.Log("Jumping -> Walling");
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Landing(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.landing_isEntering) {
                fsm.landing_isEntering = false;
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Jump
            bool succ = GameRoleDomain.Condition_InputJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterJumping();
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Walling(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.walling_isEntering) {
                fsm.walling_isEntering = false;
            }

            // Fall
            bool succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                // Debug.Log("Walling -> Landing");
                return;
            }

            bool holdWall = GameRoleDomain.Condition_InputHoldingWall(ctx, role, fsm.walling_dir, fixdt);

            // Hit Wall
            bool hitWall = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (!hitWall) {
                fsm.EnterAiring();
                // Debug.Log("Walling -> Airing");
                return;
            }

            // Wall Jump
            succ = GameRoleDomain.Condition_InputWallJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWallJumping(-wallDir, role.wallJumpDuration);
                // Debug.Log("Walling -> WallJumping: " + -wallDir);
                return;
            }

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            var walling_friction = holdWall ? wallFriction : 0f;
            GameRoleDomain.ApplyFalling(ctx, role, walling_friction, fixdt);
        }

        static void FixedTickFSM_WallJumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.wallJumping_isEntering) {
                fsm.wallJumping_isEntering = false;

                var input = ctx.inputEntity;
                input.ResetJumpAxisTemp();

                GameRoleDomain.ApplyWallJump(ctx, role, fsm.wallJumping_jumpingDir);
                return;
            }

            GameRoleDomain.ApplyWallJumpForce(ctx, role, fsm.wallJumping_jumpingDir, fixdt);

            // Hit Wall
            bool succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ && wallDir.normalized != role.inputCom.moveAxis.normalized) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                // Debug.Log("WallJumping -> Walling");
                return;
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                // Debug.Log("WallJumping -> Landing");
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

            // Time Is End
            var timeIsOver = GameRoleDomain.Condition_WallJumpingIsEnd(ctx, role, fixdt);
            if (timeIsOver) {
                fsm.EnterAiring();
                // Debug.Log("WallJumping -> Airing");
                return;
            }

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Dying(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.dying_isEntering) {
                fsm.dying_isEntering = false;
            }

            // VFX
            VFXApp.AddVFXToWorld(ctx.vfxContext, role.deadVFXName, role.deadVFXDuration, role.Pos);

            // Camera
            CameraApp.ShakeOnce(ctx.cameraContext, ctx.cameraContext.mainCameraID);
            role.needTearDown = true;
        }

    }

}