using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Leap {

    public static class TemplateInfra {

        public static async Task LoadAssets(TemplateInfraContext ctx) {

            {
                var handle = Addressables.LoadAssetAsync<GameConfig>("TM_Config");
                var cotmfig = await handle.Task;
                ctx.Config_Set(cotmfig);
                ctx.configHandle = handle;
            }

            {
                var handle = Addressables.LoadAssetsAsync<MapTM>("TM_Map", null);
                var mapList = await handle.Task;
                foreach (var tm in mapList) {
                    ctx.Map_Add(tm);
                }
                ctx.mapHandle = handle;
            }

            {
                var handle = Addressables.LoadAssetsAsync<RoleTM>("TM_Role", null);
                var roleList = await handle.Task;
                foreach (var tm in roleList) {
                    ctx.Role_Add(tm);
                }
                ctx.roleHandle = handle;
            }

            {
                var handle = Addressables.LoadAssetsAsync<BlockTM>("TM_Block", null);
                var blockList = await handle.Task;
                foreach (var tm in blockList) {
                    ctx.Block_Add(tm);
                }
                ctx.blockHandle = handle;
            }

            {
                var handle = Addressables.LoadAssetsAsync<SpikeTM>("TM_Spike", null);
                var spikeList = await handle.Task;
                foreach (var tm in spikeList) {
                    ctx.Spike_Add(tm);
                }
                ctx.spikeHandle = handle;
            }

            {
                var handle = Addressables.LoadAssetsAsync<TerrainTM>("TM_Terrain", null);
                var terrainList = await handle.Task;
                foreach (var tm in terrainList) {
                    ctx.Terrain_Add(tm);
                }
                ctx.terrainHandle = handle;

            }

        }

        public static void Release(TemplateInfraContext ctx) {
            if (ctx.configHandle.IsValid()) {
                Addressables.Release(ctx.configHandle);
            }
            if (ctx.mapHandle.IsValid()) {
                Addressables.Release(ctx.mapHandle);
            }
            if (ctx.roleHandle.IsValid()) {
                Addressables.Release(ctx.roleHandle);
            }
            if (ctx.blockHandle.IsValid()) {
                Addressables.Release(ctx.blockHandle);
            }
            if (ctx.spikeHandle.IsValid()) {
                Addressables.Release(ctx.spikeHandle);
            }
            if (ctx.terrainHandle.IsValid()) {
                Addressables.Release(ctx.terrainHandle);
            }
        }

    }

}