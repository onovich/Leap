using Codice.Client.BaseCommands.Config;
using UnityEngine;

namespace Leap {

    public static class GameRoleFSMController {

        public static void FixedTickFSM(GameBusinessContext ctx, RoleEntity role, float fixdt) {

            FixedTickFSM_Any(ctx, role, fixdt);

            RoleFSMStatus status = role.FSM_GetStatus();
            if (status == RoleFSMStatus.Landing) {
                FixedTickFSM_Landing(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Airing) {
                FixedTickFSM_Airing(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Jumping) {
                FixedTickFSM_Jumping(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Walling) {
                FixedTickFSM_Walling(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.WallJumping) {
                FixedTickFSM_WallJumping(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Dying) {
                FixedTickFSM_Dying(ctx, role, fixdt);
            } else {
                GLog.LogError($"GameRoleFSMController.FixedTickFSM: unknown status: {status}");
            }

        }

        static void FixedTickFSM_Any(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.status == RoleFSMStatus.Landing) {
                role.Color_SetColor(Color.white);
                role.gizmosColor = Color.red;
                role.gizmosText = "陆";
            } else if (fsm.status == RoleFSMStatus.Walling) {
                role.Color_SetColor(Color.red);
                role.gizmosColor = Color.white;
                role.gizmosText = "墙";
            } else if (fsm.status == RoleFSMStatus.Airing) {
                role.Color_SetColor(Color.green);
                role.gizmosColor = Color.white;
                role.gizmosText = "空";
            } else if (fsm.status == RoleFSMStatus.Jumping) {
                role.Color_SetColor(Color.yellow);
                role.gizmosColor = Color.white;
                role.gizmosText = "跳";
            } else if (fsm.status == RoleFSMStatus.WallJumping) {
                role.Color_SetColor(Color.blue);
                role.gizmosColor = Color.white;
                role.gizmosText = "蹬";
            } else if (fsm.status == RoleFSMStatus.Dying) {
                role.Color_SetColor(Color.black);
                role.gizmosColor = Color.white;
                role.gizmosText = "亡";
            }
        }

        static void FixedTickFSM_Airing(GameBusinessContext ctx, RoleEntity role, float dt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.airing_isEntering) {
                fsm.airing_isEntering = false;
                role.Color_SetColor(Color.green);
            }

            // Fall
            bool succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, dt);

            // Hit Wall
            succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction);
            if (succ) {
                fsm.EnterWalling(role.Velocity.normalized, wallFriction, role.wallingDuration);
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, dt);

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
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Hit Wall
            succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction);
            if (succ) {
                fsm.EnterWalling(role.Velocity.normalized, wallFriction, role.wallingDuration);
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

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

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Walling(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();

            // Air
            bool succ = GameRoleDomain.Condition_InputHoldingWall(ctx, role, fsm.walling_dir, fixdt);
            if (!succ) {
                fsm.EnterAiring();
                return;
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                return;
            }

            // Wall Jump
            succ = GameRoleDomain.Condition_InputWallJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWallJumping(role.inputCom.moveAxis, role.wallJumpDuration);
                return;
            }

            GameRoleDomain.ApplyFalling(ctx, role, fsm.walling_friction, fixdt);
        }

        static void FixedTickFSM_WallJumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.wallJumping_isEntering) {
                fsm.wallJumping_isEntering = false;
                role.Color_SetColor(Color.red);

                var input = ctx.inputEntity;
                input.ResetJumpAxisTemp();

                GameRoleDomain.ApplyWallJump(ctx, role, fsm.wallJumping_jumpingDir);
            }

            GameRoleDomain.ApplyWallJumpForce(ctx, role, fsm.wallJumping_jumpingDir, fixdt);

            // Hit Wall
            bool succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction);
            if (succ) {
                fsm.EnterWalling(role.Velocity.normalized, wallFriction, role.wallingDuration);
                return;
            }

            // Air
            var timeIsOver = GameRoleDomain.Condition_WallJumpingIsEnd(ctx, role, fixdt);
            if (timeIsOver) {
                fsm.EnterAiring();
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding();
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

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