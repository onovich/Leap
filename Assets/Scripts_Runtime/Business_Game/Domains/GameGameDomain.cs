using MortiseFrame.Swing;
using TenonKit.Prism;
using UnityEngine;

namespace Leap {

    public static class GameGameDomain {

        public static void NewGame(GameBusinessContext ctx) {

            var config = ctx.templateInfraContext.Config_Get();

            // Game
            var game = ctx.gameEntity;
            game.fsmComponent.Gaming_Enter();

            // Map
            var mapTypeID = config.originalMapTypeID;
            var map = GameMapDomain.Spawn(ctx, mapTypeID);
            var has = ctx.templateInfraContext.Map_TryGet(mapTypeID, out var mapTM);
            if (!has) {
                GLog.LogError($"MapTM Not Found {mapTypeID}");
            }

            // - Terrain
            var terrainPosArr = mapTM.terrainSpawnPosArr;
            var terrainTypeIDArr = mapTM.terrainTypeIDArr;
            GameMapDomain.Terrain_SetAll(ctx, map, terrainPosArr, terrainTypeIDArr);

            // Role
            var player = ctx.playerEntity;

            // - Owner
            var spawnPoint = mapTM.SpawnPoint;
            var owner = GameRoleDomain.Spawn(ctx,
                                             config.ownerRoleTypeID,
                                             spawnPoint);
            player.ownerRoleEntityID = owner.entityID;
            ctx.ownerSpawnPoint = spawnPoint;

            // Block
            var blockTMArr = mapTM.blockSpawnArr;
            var blockPosArr = mapTM.blockSpawnPosArr;
            var blockSizeArr = mapTM.blockSpawnSizeArr;
            var blockOffsetArr = mapTM.blockSpawnOffsetArr;
            var blockIndexArr = mapTM.blockSpawnIndexArr;
            GameBlockDomain.SpawnAll(ctx, blockTMArr, blockPosArr, blockSizeArr, blockOffsetArr, blockIndexArr);

            // Spike
            var spikeTMArr = mapTM.spikeSpawnArr;
            var spikePosArr = mapTM.spikeSpawnPosArr;
            var spikeSizeArr = mapTM.spikeSpawnSizeArr;
            var spikeOffsetArr = mapTM.spikeSpawnOffsetArr;
            var spikeRotationZArr = mapTM.spikeSpawnRotationZArr;
            var spikeIndexArr = mapTM.spikeSpawnIndexArr;
            GameSpikeDomain.SpawnAll(ctx, spikeTMArr, spikePosArr, spikeSizeArr, spikeOffsetArr, spikeRotationZArr, spikeIndexArr);

            // Camera
            CameraApp.Init(ctx.cameraContext, owner.transform, owner.Pos, mapTM.cameraConfinerWorldMax, mapTM.cameraConfinerWorldMin);

            // UI

            // Cursor

        }

        public static void ApplyRestartGame(GameBusinessContext ctx) {

            var spawnPoint = ctx.ownerSpawnPoint;
            var game = ctx.gameEntity;
            var enterTime = game.fsmComponent.gameOver_enterTime;
            var gameOver_isEntering = game.fsmComponent.gameOver_isEntering;

            if (gameOver_isEntering) {
                game.fsmComponent.gameOver_isEntering = false;
                CameraApp.SetMoveToTarget(ctx.cameraContext, spawnPoint, enterTime, EasingType.Linear, EasingMode.None, onComplete: () => {
                    ExitGame(ctx);
                    NewGame(ctx);
                    game.fsmComponent.Gaming_Enter();
                });
            }

        }

        public static void ApplyGameResult(GameBusinessContext ctx) {
            var owner = ctx.Role_GetOwner();
            var game = ctx.gameEntity;
            var config = ctx.templateInfraContext.Config_Get();
            if (owner == null || owner.needTearDown) {
                game.fsmComponent.GameOver_Enter(config.gameResetEnterTime);
            }
        }

        public static void ExitGame(GameBusinessContext ctx) {
            // Game
            var game = ctx.gameEntity;
            game.fsmComponent.NotInGame_Enter();

            // Map
            GameMapDomain.UnSpawn(ctx);

            // Role
            int roleLen = ctx.roleRepo.TakeAll(out var roleArr);
            for (int i = 0; i < roleLen; i++) {
                var role = roleArr[i];
                GameRoleDomain.UnSpawn(ctx, role);
            }

            // Block
            int blockLen = ctx.blockRepo.TakeAll(out var blockArr);
            for (int i = 0; i < blockLen; i++) {
                var block = blockArr[i];
                GameBlockDomain.UnSpawn(ctx, block);
            }

            // Spike
            int spikeLen = ctx.spikeRepo.TakeAll(out var spikeArr);
            for (int i = 0; i < spikeLen; i++) {
                var spike = spikeArr[i];
                GameSpikeDomain.UnSpawn(ctx, spike);
            }

            // UI
        }

    }
}