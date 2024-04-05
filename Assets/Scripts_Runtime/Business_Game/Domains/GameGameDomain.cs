using UnityEngine;

namespace Leap {

    public static class GameGameDomain {

        public static void NewGame(GameBusinessContext ctx) {

            var config = ctx.templateInfraContext.Config_Get();

            // Game
            var game = ctx.gameEntity;
            game.fsmComponent.Gaming_Enter();

            // Map
            var map = GameMapDomain.Spawn(ctx, 1);
            var has = ctx.templateInfraContext.Map_TryGet(1, out var mapTM);
            if (!has) {
                GLog.LogError($"MapTM Not Found {1}");
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

            // Camera

            // UI

            // Cursor

        }

        public static void ExitGame(GameBusinessContext ctx) {
            // Game
            var game = ctx.gameEntity;
            game.fsmComponent.NotInGame_Enter();

            // Map
            GameMapDomain.UnSpawn(ctx);

            // Role
            int roleLen = ctx.roleRepo.TakeAll(out var roles);
            for (int i = 0; i < roleLen; i++) {
                var role = roles[i];
                GameRoleDomain.UnSpawn(ctx, role);
            }

            // Block
            int blockLen = ctx.blockRepo.TakeAll(out var blocks);
            for (int i = 0; i < blockLen; i++) {
                var block = blocks[i];
                GameBlockDomain.UnSpawn(ctx, block);
            }

            // UI
        }

    }
}