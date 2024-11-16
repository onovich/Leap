using UnityEngine;

namespace Leap {

    public static class GameBusiness {

        public static void Init(GameBusinessContext ctx) {
            Physics2D.IgnoreLayerCollision(LayConst.ROLE, LayConst.ROLE, true);
        }

        public static void StartGame(GameBusinessContext ctx) {
            GameGameDomain.NewGame(ctx);
        }

        public static void ExitGame(GameBusinessContext ctx) {
            GameGameDomain.ExitGame(ctx);
        }

        public static void Tick(GameBusinessContext ctx, float dt) {

            InputEntity inputEntity = ctx.inputEntity;

            ProcessInput(ctx, dt);
            PreTick(ctx, dt);

            const float intervalTime = 0.01f;
            ref float restSec = ref ctx.fixedRestSec;
            restSec += dt;
            if (restSec < intervalTime) {
                FixedTick(ctx, restSec);
                restSec = 0;
            } else {
                while (restSec >= intervalTime) {
                    restSec -= intervalTime;
                    FixedTick(ctx, intervalTime);
                }
            }
            LateTick(ctx, dt);
            inputEntity.Reset();

        }

        public static void ProcessInput(GameBusinessContext ctx, float dt) {
            GameInputDomain.Player_BakeInput(ctx, dt);

            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {
                GameInputDomain.Owner_BakeInput(ctx, ctx.Role_GetOwner());
            }
        }

        static void PreTick(GameBusinessContext ctx, float dt) {
            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {

                // Roles
                var roleLen = ctx.roleRepo.TakeAll(out var roleArr);
                for (int i = 0; i < roleLen; i++) {
                    var role = roleArr[i];
                    GameRoleDomain.CheckAndUnSpawn(ctx, role);
                }

                // Result
                GameGameDomain.ApplyGameResult(ctx);
            }
            if (status == GameStatus.GameOver) {
                GameGameDomain.ApplyRestartGame(ctx);
            }
        }

        static void FixedTick(GameBusinessContext ctx, float fixdt) {

            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {

                // Roles
                var roleLen = ctx.roleRepo.TakeAll(out var roleArr);
                for (int i = 0; i < roleLen; i++) {
                    var role = roleArr[i];
                    GameRoleFSMController.FixedTickFSM(ctx, role, fixdt);
                }

                for (int i = 0; i < roleLen; i++) {
                    var role = roleArr[i];
                    // GameRoleDomain.Physics_ResetHitSpike(ctx, role);
                    // GameRoleDomain.Physics_ResetHitWall(ctx, role);
                    // GameRoleDomain.Physics_ResetLandGround(ctx, role);
                }

                Physics2D.Simulate(fixdt);

                for (int i = 0; i < roleLen; i++) {
                    var role = roleArr[i];
                    GameRoleDomain.Physics_CheckLandGround(ctx, role);
                    GameRoleDomain.Physics_CheckHitSpike(ctx, role);
                    GameRoleDomain.Physics_CheckHitWall(ctx, role);
                }
            }

        }

        static void LateTick(GameBusinessContext ctx, float dt) {

            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            var owner = ctx.Role_GetOwner();
            if (status == GameStatus.Gaming || status == GameStatus.GameOver) {

                // Camera
                CameraApp.LateTick(ctx.cameraContext, dt);

                // UI

            }
            // VFX
            VFXApp.LateTick(ctx.vfxContext, dt);
        }

        public static void TearDown(GameBusinessContext ctx) {
            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {
                ExitGame(ctx);
            }
        }

#if UNITY_EDITOR
        public static void OnDrawGizmos(GameBusinessContext ctx, bool drawCameraGizmos) {
            if (ctx == null) {
                return;
            }
            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {
                if (drawCameraGizmos) {
                    CameraApp.OnDrawGizmos(ctx.cameraContext);

                    var roleLen = ctx.roleRepo.TakeAll(out var roleArr);
                    for (int i = 0; i < roleLen; i++) {
                        var role = roleArr[i];
                        GameRoleDomain.OnDrawGizmos(ctx, role);
                    }
                }
            }
        }
#endif

    }

}