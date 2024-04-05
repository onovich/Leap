using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Leap {

    public class TemplateInfraContext {

        GameConfig config;
        public AsyncOperationHandle configHandle;

        Dictionary<int, MapTM> mapDict;
        public AsyncOperationHandle mapHandle;

        Dictionary<int, RoleTM> roleDict;
        public AsyncOperationHandle roleHandle;

        Dictionary<int, BlockTM> blockDict;
        public AsyncOperationHandle blockHandle;

        public TemplateInfraContext() {
            mapDict = new Dictionary<int, MapTM>();
            roleDict = new Dictionary<int, RoleTM>();
            blockDict = new Dictionary<int, BlockTM>();
        }

        // Game
        public void Config_Set(GameConfig config) {
            this.config = config;
        }

        public GameConfig Config_Get() {
            return config;
        }

        // Map
        public void Map_Add(MapTM map) {
            mapDict.Add(map.typeID, map);
        }

        public bool Map_TryGet(int typeID, out MapTM map) {
            var has = mapDict.TryGetValue(typeID, out map);
            if (!has) {
                GLog.LogError($"Map {typeID} not found");
            }
            return has;
        }

        // Role
        public void Role_Add(RoleTM role) {
            roleDict.Add(role.typeID, role);
        }

        public bool Role_TryGet(int typeID, out RoleTM role) {
            var has = roleDict.TryGetValue(typeID, out role);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }
            return has;
        }

        // Block
        public void Block_Add(BlockTM block) {
            blockDict.Add(block.typeID, block);
        }

        public bool Block_TryGet(int typeID, out BlockTM block) {
            var has = blockDict.TryGetValue(typeID, out block);
            if (!has) {
                GLog.LogError($"Block {typeID} not found");
            }
            return has;
        }

        // Clear
        public void Clear() {
            mapDict.Clear();
            roleDict.Clear();
            blockDict.Clear();
        }

    }

}