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
            } else if (status == RoleFSMStatus.Dash) {
                FixedTickFSM_Dash(ctx, role, fixdt);
                SetFSMGizmos(ctx, role, Color.magenta, Color.white, "冲");
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

        static void FixedTickFSM_Airing(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.airing_isEntering) {
                fsm.airing_isEntering = false;
                role.Move_Stop();
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Dash
            bool succ = GameRoleDomain.Condition_InputDash(ctx, role, fixdt);
            if (succ) {
                fsm.EnterDash(role.inputCom.dashAxis, role.dashDuration);
                // Debug.Log("Walling -> Dash");
                return;
            }


            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding(role.landDuration);
                return;
            }
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            // Hit Wall
            succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                return;
            }

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

        static void FixedTickFSM_Jumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            var input = ctx.inputEntity;
            if (fsm.jumping_isEntering) {
                fsm.jumping_isEntering = false;
                input.ResetJumpAxisTemp();
                GameRoleDomain.ApplyJump(ctx, role, fixdt);
                return;
            }

            // Dash
            bool succ = GameRoleDomain.Condition_InputDash(ctx, role, fixdt);
            if (succ) {
                fsm.EnterDash(role.inputCom.dashAxis, role.dashDuration);
                // Debug.Log("Walling -> Dash");
                return;
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding(role.landDuration);
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
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            // Dash
            bool succ = GameRoleDomain.Condition_InputDash(ctx, role, fixdt);
            if (succ) {
                fsm.EnterDash(role.inputCom.dashAxis, role.dashDuration);
                // Debug.Log("Walling -> Dash");
                return;
            }

            // Fall
            GameRoleDomain.ApplyFalling(ctx, role, 0f, fixdt);

            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (!succ) {
                if (fsm.LandingTimerIsEnd(fixdt)) {
                    fsm.EnterAiring();
                    // Debug.Log("Walling -> Landing");
                    return;
                }
            } else {
                fsm.ResetLandingTimer();
            }

            // Jump
            succ = GameRoleDomain.Condition_InputJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterJumping();
                // Debug.Log("Landing -> Jumping");
                return;
            }

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
                role.Move_Stop();

                // Wall Jump
                bool inputWallJump = GameRoleDomain.Condition_InputWallJump(ctx, role, fixdt);
                if (inputWallJump) {
                    fsm.EnterWallJumping(-fsm.walling_dir, role.wallJumpDuration);
                    // Debug.Log("Walling -> WallJumping: " + -wallDir);
                    return;
                }

                return;
            }

            // Dash
            bool succ = GameRoleDomain.Condition_InputDash(ctx, role, fixdt);
            if (succ) {
                fsm.EnterDash(role.inputCom.dashAxis, role.dashDuration);
                // Debug.Log("Walling -> Dash");
                return;
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding(role.landDuration);
                // Debug.Log("Walling -> Landing");
                return;
            }

            // Move
            GameRoleDomain.ApplyMove(ctx, role, fixdt);

            bool holdWall = GameRoleDomain.Condition_InputHoldingWall(ctx, role, fsm.walling_dir, fixdt);

            // Hit Wall
            bool hitWall = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (hitWall) {
                fsm.ResetWallingTimer();
            } else {
                if (fsm.WallingTimerIsEnd(fixdt)) {
                    fsm.EnterAiring();
                    return;
                }
            }

            // Wall Jump
            succ = GameRoleDomain.Condition_InputWallJump(ctx, role, fixdt);
            if (succ) {
                fsm.EnterWallJumping(-fsm.walling_dir, role.wallJumpDuration);
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

        static void FixedTickFSM_Dash(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.dash_isEntering) {
                fsm.dash_isEntering = false;
                role.Move_Stop();
                GameRoleDomain.ApplyDash(ctx, role, fsm.dash_dir);
                return;
            }

            GameRoleDomain.ApplyDashForce(ctx, role, fsm.dash_dir, fixdt);

            // Hit Wall
            bool succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ && wallDir.normalized != role.lastFaceDir) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                // Debug.Log("WallJumping -> Walling");
                return;
            }

            // Spike
            succ = GameRoleDomain.Condition_CheckHitSpike(ctx, role);
            if (succ) {
                GameRoleDomain.GetDeadlyHurt(ctx, role);
            }

            // Constraint
            GameRoleDomain.ApplyConstraint(ctx, role, fixdt);

            // Time Is End
            var timeIsOver = GameRoleDomain.Condition_DashIsEnd(ctx, role, fixdt);
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

        static void FixedTickFSM_WallJumping(GameBusinessContext ctx, RoleEntity role, float fixdt) {
            RoleFSMComponent fsm = role.FSM_GetComponent();
            if (fsm.wallJumping_isEntering) {
                fsm.wallJumping_isEntering = false;
                role.Move_Stop();

                var input = ctx.inputEntity;
                input.ResetJumpAxisTemp();

                GameRoleDomain.ApplyWallJump(ctx, role, fsm.wallJumping_jumpingDir);
                return;
            }

            GameRoleDomain.ApplyWallJumpForce(ctx, role, fsm.wallJumping_jumpingDir, fixdt);

            // Hit Wall
            bool succ = GameRoleDomain.Condition_CheckHitWall(ctx, role, out var wallFriction, out var wallDir);
            if (succ && wallDir.normalized != role.lastFaceDir) {
                fsm.EnterWalling(wallDir, role.wallingDuration);
                // Debug.Log("WallJumping -> Walling");
                return;
            }

            // Dash
            succ = GameRoleDomain.Condition_InputDash(ctx, role, fixdt);
            if (succ) {
                fsm.EnterDash(role.inputCom.dashAxis, role.dashDuration);
                // Debug.Log("Walling -> Dash");
                return;
            }

            // Fall
            succ = GameRoleDomain.Condition_CheckLandGround(ctx, role);
            if (succ) {
                fsm.EnterLanding(role.landDuration);
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
                return;
            }

            // VFX
            VFXApp.AddVFXToWorld(ctx.vfxContext, role.deadVFXName, role.deadVFXDuration, role.Pos);

            // Camera
            CameraApp.ShakeOnce(ctx.cameraContext, ctx.cameraContext.mainCameraID);
            role.needTearDown = true;
        }

    }

}