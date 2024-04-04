using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Leap {

    public class TemplateInfraContext {

        GameConfig config;
        public AsyncOperationHandle configHandle;

        Dictionary<int, RoleTM> roleDict;
        public AsyncOperationHandle roleHandle;

        public TemplateInfraContext() {
            roleDict = new Dictionary<int, RoleTM>();
        }

        // Game
        public void Config_Set(GameConfig config) {
            this.config = config;
        }

        public GameConfig Config_Get() {
            return config;
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

        // Clear
        public void Clear() {
            roleDict.Clear();
        }

    }

}