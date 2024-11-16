using UnityEngine;

namespace Leap {

    public static class GameSpikeDomain {

        public static SpikeEntity Spawn(GameBusinessContext ctx, int typeID, Vector2Int pos, Vector2 size, Vector2 offset, int rotationZ, int index) {
            var spike = GameFactory.Spike_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos,
                                              size,
                                              offset,
                                              rotationZ,
                                              index);
            ctx.spikeRepo.Add(spike);
            return spike;
        }

        public static void SpawnAll(GameBusinessContext ctx, SpikeTM[] blockTMArr, Vector2Int[] posArr, Vector2[] sizeArr, Vector2[] offsetArr, int[] rotationZArr, int[] indexArr) {
            for (int i = 0; i < blockTMArr.Length; i++) {
                var tm = blockTMArr[i];
                var pos = posArr[i];
                var size = sizeArr[i];
                var offset = offsetArr[i];
                var rotationZ = rotationZArr[i];
                var index = indexArr[i];
                Spawn(ctx, tm.typeID, pos, size, offset, rotationZ, index);
            }
        }

        public static void UnSpawn(GameBusinessContext ctx, SpikeEntity spike) {
            ctx.spikeRepo.Remove(spike);
            spike.TearDown();
        }

    }

}