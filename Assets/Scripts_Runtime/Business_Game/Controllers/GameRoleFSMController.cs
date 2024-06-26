using Codice.Client.BaseCommands.Config;
using UnityEngine;

namespace Leap {

    public static class GameRoleFSMController {

        public static void FixedTickFSM(GameBusinessContext ctx, RoleEntity role, float fixdt) {

            FixedTickFSM_Any(ctx, role, fixdt);

            RoleFSMStatus status = role.FSM_GetStatus();
            if (status == RoleFSMStatus.Normal) {
                FixedTickFSM_Idle(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.WallJumping) {
                FixedTickFSM_WallJumping(ctx, role, fixdt);
            } else if (status == RoleFSMStatus.Dead) {
                FixedTickFSM_Dead(ctx, role, fixdt);
            } else {
                GLog.LogError($"GameRoleFSMController.FixedTickFSM: unknown status: {status}");
            }

        }

        static void FixedTickFSM_Any(GameBusinessContext ctx, RoleEntity role, float fixdt) {

        }

        static void FixedTickFSM_Idle(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.normal_isEntering) {
                fsm.normal_isEntering = false;
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
            bool succ = GameRoleDomain.ApplyTryWallJump(ctx, role, fixdt);
            if (succ) {
                return;
            }

            // Jump
            succ = GameRoleDomain.ApplyTryJump(ctx, role, fixdt);

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDead();
            }
        }

        static void FixedTickFSM_WallJumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.wallJumping_isEntering) {
                fsm.wallJumping_isEntering = false;
                role.Color_SetColor(Color.red);
            }

            // Wall Jump
            var timeIsOver = GameRoleDomain.ApplyWallJump(ctx, role, fixdt);
            if (timeIsOver) {
                fsm.EnterNormal();
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Hit Wall
            GameRoleDomain.ApplyHitWall(ctx, role, fixdt);

            // Dead
            if (role.hp <= 0) {
                fsm.EnterDead();
            }
        }

        static void FixedTickFSM_Dead(GameBusinessContext ctx, RoleEntity role, float fixdt) {
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