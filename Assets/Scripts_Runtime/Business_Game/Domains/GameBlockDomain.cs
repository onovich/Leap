using UnityEngine;

namespace Leap {

    public static class GameBlockDomain {

        public static BlockEntity Spawn(GameBusinessContext ctx, int typeID, Vector2Int pos, Vector2Int size) {
            var block = GameFactory.Block_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos,
                                              size);
            ctx.blockRepo.Add(block);
            return block;
        }

        public static void SpawnAll(GameBusinessContext ctx, BlockTM[] blockTMArr, Vector2Int[] posArr, Vector2Int[] sizeArr) {
            for (int i = 0; i < blockTMArr.Length; i++) {
                var tm = blockTMArr[i];
                var pos = posArr[i];
                var size = sizeArr[i];
                Spawn(ctx, tm.typeID, pos, size);
            }
        }

        public static void UnSpawn(GameBusinessContext ctx, BlockEntity block) {
            ctx.blockRepo.Remove(block);
            block.TearDown();
        }

    }

}