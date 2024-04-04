using UnityEngine;

namespace Leap {

    public static class GameBusiness {

        public static void Init(GameBusinessContext ctx) {

        }

        public static void StartGame(GameBusinessContext ctx) {
            GameGameDomain.NewGame(ctx);
        }

        public static void ExitGame(GameBusinessContext ctx) {
            GameGameDomain.ExitGame(ctx);
        }

        public static void Tick(GameBusinessContext ctx, float dt) {

            InputEntity inputEntity = ctx.inputEntity;
            inputEntity.Reset();

            ProcessInput(ctx, dt);
            PreTick(ctx, dt);

            float restTime = dt;
            const float intervalTime = 0.01f;
            for (; restTime >= intervalTime; restTime -= intervalTime) {
                FixedTick(ctx, intervalTime);
            }
            if (restTime > 0) {
                FixedTick(ctx, restTime);
            }
            LateTick(ctx, dt);

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
            }
        }

        static void FixedTick(GameBusinessContext ctx, float fixdt) {

            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            if (status == GameStatus.Gaming) {

                // Roles
                ctx.roleRepo.ForEach((role) => {
                    GameRoleFSMController.FixedTickFSM(ctx, role, fixdt);
                });

                Physics2D.Simulate(fixdt);
            }
        }

        static void LateTick(GameBusinessContext ctx, float dt) {

            var game = ctx.gameEntity;
            var status = game.fsmComponent.status;
            var owner = ctx.Role_GetOwner();
            if (status == GameStatus.Gaming) {

                // Camera

                // UI
            }
        }

        public static void TearDown(GameBusinessContext ctx) {
            ExitGame(ctx);
        }

        // UI

    }

}