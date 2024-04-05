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
            GameMapDomain.Terrain_SetAll(ctx, map, terrainPosArr);

            // Role
            var player = ctx.playerEntity;

            // - Owner
            var owner = GameRoleDomain.Spawn(ctx,
                                             config.ownerRoleTypeID,
                                             new Vector2(0, 0));
            player.ownerRoleEntityID = owner.entityID;

            // Block
            var blockTMArr = mapTM.blockSpawnArr;
            var blockPosArr = mapTM.blockSpawnPosArr;
            var blockSizeArr = mapTM.blockSpawnSizeArr;
            GameBlockDomain.SpawnAll(ctx, blockTMArr, blockPosArr, blockSizeArr);

            // Spike
            var spikeTMArr = mapTM.spikeSpawnArr;
            var spikePosArr = mapTM.spikeSpawnPosArr;
            var spikeSizeArr = mapTM.spikeSpawnSizeArr;
            var spikeRotationZArr = mapTM.spikeSpawnRotationZArr;
            GameSpikeDomain.SpawnAll(ctx, spikeTMArr, spikePosArr, spikeSizeArr, spikeRotationZArr);

            // Camera

            // UI

            // Cursor

        }

        public static void ApplyGameResult(GameBusinessContext ctx) {
            var owner = ctx.Role_GetOwner();
            var game = ctx.gameEntity;
            if (owner == null || owner.needTearDown) {
                game.fsmComponent.NotInGame_Enter();
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