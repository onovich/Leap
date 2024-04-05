using UnityEngine;

namespace Leap {

    public static class GameBlockDomain {

        public static BlockEntity Spawn(GameBusinessContext ctx, int typeID, Vector2 pos) {
            var block = GameFactory.Block_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos);
            ctx.blockRepo.Add(block);
            return block;
        }

        public static GameObject SpawnMulti(GameBusinessContext ctx, int typeID, Vector2 pos) {
            var block = GameFactory.Block_SpawnMulti(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos);
            return block;
        }

        public static void UnSpawn(GameBusinessContext ctx, RoleEntity role) {
            ctx.roleRepo.Remove(role);
            role.TearDown();
        }

        public static void ApplyMove(GameBusinessContext ctx, RoleEntity role, float dt) {

            var player = ctx.playerEntity;
            var owner = ctx.Role_GetOwner();

            if (owner.inputCom.moveAxis == Vector2.zero) {
                role.Move_Stop();
            } else if (owner.inputCom.moveAxis != Vector2.zero) {
                role.Move_ApplyMove(dt);
            }

        }

    }

}