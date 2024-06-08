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

            if (role.isWall) {
                role.Color_SetColor(Color.green);
            } else {
                role.Color_SetColor(Color.yellow);
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Jump
            bool succ = GameRoleDomain.ApplyTryJump(ctx, role, fixdt);

            // Hold Wall
            GameRoleDomain.ApplyHoldWall(ctx, role, fixdt);

            // Wall Jump
            succ = GameRoleDomain.ApplyTryWallJump(ctx, role, fixdt);
            if (succ) {
                return;
            }

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

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, fixdt);

            // Hit Wall
            GameRoleDomain.ApplyHitWall(ctx, role, fixdt);

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