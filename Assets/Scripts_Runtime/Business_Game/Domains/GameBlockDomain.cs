using UnityEngine;

namespace Leap {

    public static class GameBlockDomain {

        public static BlockEntity Spawn(GameBusinessContext ctx, int typeID, Vector2Int pos, Vector2Int size, int index) {
            var block = GameFactory.Block_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos,
                                              size,
                                              index);
            ctx.blockRepo.Add(block);
            return block;
        }

        public static void SpawnAll(GameBusinessContext ctx, BlockTM[] blockTMArr, Vector2Int[] posArr, Vector2Int[] sizeArr, int[] indexArr) {
            for (int i = 0; i < blockTMArr.Length; i++) {
                var tm = blockTMArr[i];
                var pos = posArr[i];
                var size = sizeArr[i];
                var index = indexArr[i];
                Spawn(ctx, tm.typeID, pos, size, index);
            }
        }

        public static void UnSpawn(GameBusinessContext ctx, BlockEntity block) {
            ctx.blockRepo.Remove(block);
            block.TearDown();
        }

        public static float GetFallingFriction(GameBusinessContext ctx, int typeID) {
            var has = ctx.templateInfraContext.Block_TryGet(typeID, out var blockTM);
            if (!has) {
                GLog.LogError($"BlockTM Not Found At {typeID}");
            }
            return blockTM.fallingFriction;
        }

    }

}