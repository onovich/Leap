using UnityEngine;
using UnityEngine.Tilemaps;

namespace Leap {

    public static class GameMapDomain {

        public static MapEntity Spawn(GameBusinessContext ctx, int typeID) {
            var map = GameFactory.Map_Spawn(ctx.templateInfraContext, ctx.assetsInfraContext, typeID);
            ctx.currentMapEntity = map;
            return map;
        }

        public static void UnSpawn(GameBusinessContext ctx) {
            var map = ctx.currentMapEntity;
            Terrain_ClearAll(ctx, map);
            map.TearDown();
            ctx.currentMapEntity = null;
        }

        static void Terrain_ClearAll(GameBusinessContext ctx, MapEntity map) {
            map.Terrain_ClearAll();
        }

        public static void Terrain_SetAll(GameBusinessContext ctx, MapEntity map, Vector2Int[] posArr, int[] typeIDArr) {
            var tile = map.tileBase_terrain;
            for (int i = 0; i < posArr.Length; i++) {
                var pos = posArr[i];
                var typeID = typeIDArr[i];
                Terrain_Set(ctx, map, pos, typeID, tile);
            }
        }

        public static void Terrain_Set(GameBusinessContext ctx, MapEntity map, Vector2Int pos, int typeID, TileBase tile) {
            map.Terrain_Set(pos, tile, typeID);
        }

        public static float Terrain_GetFallingFriction(GameBusinessContext ctx, MapEntity map, Vector2 pos, Vector2 normal) {
            // var worldPos = pos - normal * 0.5f;
            var worldPos = pos;
            var tileMap = map.Tilemap_Terrain;
            var posInt = tileMap.WorldToCell(worldPos).ToVector2Int();
            var has = map.Terrain_GetTypeID(posInt, out var typeID);
            if (!has) {
                GLog.LogError($"TerrainTypeID Not Found At {posInt} (originalPos = {pos}); worldPos = {worldPos};   dir = {normal}");
                return 0;
            }
            has = ctx.templateInfraContext.Terrain_TryGet(typeID, out var terrainTM);
            if (!has) {
                GLog.LogError($"TerrainTM Not Found At {posInt} ({pos}) dir = {normal}");
            }
            return terrainTM.fallingFriction;
        }

    }

}