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
            map.constraintSize = mapTM.constraintSize;
            map.constraintCenter = mapTM.constraintCenter;
            map.tileBase_terrain = mapTM.tileBase_terrain;
            map.Pos_Set(mapTM.mapPos);

            return map;
        }

        public static RoleEntity Role_Spawn(TemplateInfraContext templateInfraContext,
                                 AssetsInfraContext assetsInfraContext,
                                 IDRecordService idRecordService,
                                 int typeID,
                                 Vector2 pos,
                                 bool isDebugMode) {

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
            role.jumpForceY = roleTM.jumpForceY;
            role.wallJumpForceYMax = roleTM.wallJumpForceY;
            role.wallJumpForceXMax = roleTM.wallJumpForceX;
            role.wallJumpDuration = roleTM.wallJumpDuration;
            role.wallingDuration = roleTM.wallingDuration;
            role.wallJumpAccelerationX = roleTM.wallJumpAccelerationX;
            role.wallJumpAccelerationY = roleTM.wallJumpAccelerationY;
            role.dashSpeed = roleTM.dashSpeed;
            role.dashAcceleration = roleTM.dashAcceleration;
            role.dashDuration = roleTM.dashDuration;
            role.dashForceMax = roleTM.dashForce;
            role.landDuration = roleTM.landDuration;
            role.g = roleTM.g;
            role.fallingSpeedMax = roleTM.fallingSpeedMax;
            role.hp = roleTM.hp;
            role.hpMax = roleTM.hp;

            // Set Skill
            role.hasWallJump = roleTM.hasWallJump;
            role.hasDoubleJump = roleTM.hasDoubleJump;
            role.hasDash = roleTM.hasDash;

            // Set Pos
            role.Pos_SetPos(pos);

            // Set Mesh
            var mesh = isDebugMode ? roleTM.mesh_debug : roleTM.mesh;
            role.Mesh_Set(mesh);

            // Set FSM
            role.fsmCom.EnterLanding(roleTM.landDuration);

            // Set VFX
            role.deadVFXName = roleTM.deadVFX.name;
            role.deadVFXDuration = roleTM.deadVFXDuration;

            // Set Physics
            role.Size_SetBodyCollSize(roleTM.bodyColliderSize);
            role.Size_SetHeadCollSize(roleTM.headColliderSize);

            return role;
        }

        public static BlockEntity Block_Spawn(TemplateInfraContext templateInfraContext,
                                  AssetsInfraContext assetsInfraContext,
                                  IDRecordService idRecordService,
                                  int typeID,
                                  Vector2Int pos,
                                  Vector2Int size,
                                  Vector2 offset,
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

            // Set Size
            block.Size_SetSize(size);

            // Set Mesh Offset
            block.Body_SetOffset(offset);

            // Set Pos
            block.Pos_SetPos(pos);

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
                                  Vector2 size,
                                  Vector2 offset,
                                  int rotationZ,
                                  int index) {

            var has = templateInfraContext.Spike_TryGet(typeID, out var spikeTM);
            if (!has) {
                GLog.LogError($"Role {typeID} not found");
            }

            var prefab = assetsInfraContext.Entity_GetSpike();
            var spike = GameObject.Instantiate(prefab).GetComponent<SpikeEntity>();
            spike.Ctor();

            // Base Info
            spike.entityIndex = index;
            spike.typeID = typeID;

            // Set Size
            spike.Size_SetSize(size);

            // Set Physics Size
            spike.Physics_SetColliderSize(size);

            // Set Offset
            spike.Body_SetOffset(offset);

            // Set Pos
            spike.Pos_SetPos(pos + offset);

            // Set Mesh
            spike.Mesh_Set(spikeTM.mesh);

            // Rename
            spike.Rename();

            return spike;
        }

    }

}