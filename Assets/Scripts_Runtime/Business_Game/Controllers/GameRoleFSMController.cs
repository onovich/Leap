using Codice.Client.BaseCommands.Config;
using UnityEngine;

namespace Leap {

    public static class GameRoleFSMController {

        public static void FixedTickFSM(GameBusinessContext ctx, RoleEntity role, float fixdt) {

            FixedTickFSM_Any(ctx, role, fixdt);

            RoleFSMStatus status = role.FSM_GetStatus();
            if (status == RoleFSMStatus.Walking) {
                FixedTickFSM_Walking(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Jumping) {
                FixedTickFSM_Jumping(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.WallJumping) {
                FixedTickFSM_WallJumping(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Dying) {
                FixedTickFSM_Dying(ctx, role, fixdt);
            } else {
                GLog.LogError($"GameRoleFSMController.FixedTickFSM: unknown status: {status}");
            }

        }

        static void FixedTickFSM_Any(GameBusinessContext ctx, RoleEntity role, float fixdt) {

        }

        static void FixedTickFSM_Jumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            var input = ctx.inputEntity;
            if (fsm.jumping_isEntering) {
                fsm.jumping_isEntering = false;
                input.ResetJumpAxisTemp();

                GameRoleDomain.ApplyJump(ctx, role, fixdt);
            }

            // Fall
            bool succ = GameRoleDomain.Condition_IsGround(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWalking();
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Wall Jump
            succ = GameRoleDomain.Condition_WallJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWallJumping(role.Move_GetWallJumpingDir(), role.wallJumpDuration);
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Walking(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.walking_isEntering) {
                fsm.walking_isEntering = false;
            }

            if (role.isHoldingWall()) {
                role.Color_SetColor(new Color(1, 0.5f, 0.5f));
            } else if (role.isWall) {
                role.Color_SetColor(Color.green);
            } else if (role.isGround) {
                role.Color_SetColor(Color.yellow);
            } else {
                role.Color_SetColor(Color.yellow);
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Wall Jump
            bool succ = GameRoleDomain.Condition_WallJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWallJumping(role.Move_GetWallJumpingDir(), role.wallJumpDuration);
                return;
            }

            // Jump
            succ = GameRoleDomain.Condition_Jump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterJumping();
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_WallJumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.wallJumping_isEntering) {
                fsm.wallJumping_isEntering = false;
                role.Color_SetColor(Color.red);

                var input = ctx.inputEntity;
                input.ResetJumpAxisTemp();
                role.Move_ResetEnterWallDir_Manual();

                GameRoleDomain.ApplyWallJump(ctx, role, fsm.wallJumping_jumpingDir);
            }

            // Wall Jump
            var timeIsOver = GameRoleDomain.Condition_WallJumpIsEnd(ctx, role, fixdt);
            if (timeIsOver) {
                fsm.EnterWalking();
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Hit Wall
            // GameRoleDomain.ApplyHitWall(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDying();
            }
        }

        static void FixedTickFSM_Dying(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.dead_isEntering) {
                fsm.dead_isEntering = false;
            }

            // VFX
            VFXApp.AddVFXToWorld(ctx.vfxContext, role.deadVFXName, role.deadVFXDuration, role.Pos);

            // Camera
            CameraApp.ShakeOnce(ctx.cameraContext, ctx.cameraContext.mainCameraID);
            role.needTearDown = true;
        }

    }

}