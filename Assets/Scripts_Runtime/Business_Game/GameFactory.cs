using UnityEngine;

namespace Leap {

    public static class GameFactory {

        public static MapEntity Map_Spawn(TemplateInfraContext templateInfraContext,
                                 AssetsInfraContext assetsInfraContext,
                                 int typeID) {

            var has = templateInfraContext.Map_TryGet(typeID, out var mapTM);
            if (!has) {
                GLog.LogError($"Map {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetMap();
            var map = GameObject.Instantiate(prefab).GetComponent<MapEntity>();
            map.Ctor();
            map.typeID = typeID;
            map.mapSize = mapTM.mapSize;
            map.tileBase_terrain = mapTM.tileBase_terrain;

            return map;
        }

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
            role.hp = roleTM.hp;
            role.hpMax = roleTM.hp;

            // Set Pos
            role.Pos_SetPos(pos);

            // Set Mesh
            role.Mesh_Set(roleTM.mesh);

            // Set FSM
            role.FSM_EnterIdle();

            // Set VFX
            role.deadVFXName = roleTM.deadVFX.name;
            role.deadVFXDuration = roleTM.deadVFXDuration;

            return role;
        }

        public static BlockEntity Block_Spawn(TemplateInfraContext templateInfraContext,
                                  AssetsInfraContext assetsInfraContext,
                                  IDRecordService idRecordService,
                                  int typeID,
                                  Vector2Int pos,
                                  Vector2Int size,
                                  int index) {

            var has = templateInfraContext.Block_TryGet(typeID, out var blockTM);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetBlock();
            var block = GameObject.Instantiate(prefab).GetComponent<BlockEntity>();
            block.Ctor();

            // Base Info
            block.entityIndex = index;
            block.typeID = typeID;

            // Set Pos
            block.Pos_SetPos(pos);

            // Set Size
            block.Size_SetSize(size);

            // Set Mesh
            block.Mesh_Set(blockTM.mesh);

            // Rename
            block.Rename();

            return block;
        }

        public static SpikeEntity Spike_Spawn(TemplateInfraContext templateInfraContext,
                                  AssetsInfraContext assetsInfraContext,
                                  IDRecordService idRecordService,
                                  int typeID,
                                  Vector2Int pos,
                                  Vector2Int size,
                                  int rotationZ,
                                  int index) {

            var has = templateInfraContext.Spike_TryGet(typeID, out var blockTM);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetSpike();
            var spike = GameObject.Instantiate(prefab).GetComponent<SpikeEntity>();
            spike.Ctor();

            // Base Info
            spike.entityIndex = index;
            spike.typeID = typeID;

            // Set Pos
            spike.Pos_SetPos(pos);

            // Set Size
            spike.Size_SetSize(size);

            // Set Mesh
            spike.Mesh_Set(blockTM.mesh);

            // Rename
            spike.Rename();

            return spike;
        }

    }

}