using UnityEngine;

namespace Leap {

    public static class GameFactory {

        public static RoleEntity Role_Spawn(TemplateInfraContext templateInfraContext,
                                 AssetsInfraContext assetsInfraContext,
                                 IDRecordService idRecordService,
                                 int typeID,
                                 Vector2 pos) {

            var has = templateInfraContext.Role_TryGet(typeID, out var roleTM);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetRole();
            var role = GameObject.Instantiate(prefab).GetComponent<RoleEntity>();
            role.Ctor();

            // Base Info
            role.entityID = idRecordService.PickRoleEntityID();
            role.typeID = typeID;
            role.allyStatus = roleTM.allyStatus;
            role.aiType = roleTM.aiType;

            // Set Attr
            role.moveSpeed = roleTM.moveSpeed;
            role.jumpForce = roleTM.jumpForce;
            role.g = roleTM.g;
            role.fallingSpeedMax = roleTM.fallingSpeedMax;

            // Set Pos
            role.Pos_SetPos(pos);

            // Set Mesh
            role.Mesh_Set(roleTM.mesh);

            // Set FSM
            role.FSM_EnterIdle();

            return role;
        }

        public static BlockEntity Block_Spawn(TemplateInfraContext templateInfraContext,
                                  AssetsInfraContext assetsInfraContext,
                                  IDRecordService idRecordService,
                                  int typeID,
                                  Vector2 pos) {

            var has = templateInfraContext.Block_TryGet(typeID, out var blockTM);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetBlock();
            var block = GameObject.Instantiate(prefab).GetComponent<BlockEntity>();
            block.Ctor();

            // Base Info
            block.entityID = idRecordService.PickRoleEntityID();
            block.typeID = typeID;

            // Set Pos
            block.Pos_SetPos(pos);

            // Set Mesh
            block.Mesh_Set(blockTM.mesh);

            return block;
        }

    }

}