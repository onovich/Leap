using UnityEngine;

namespace Leap {

    public static class GameRoleDomain {

        public static RoleEntity Spawn(GameBusinessContext ctx, int typeID, Vector2 pos) {
            var role = GameFactory.Role_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos);

            role.OnFootTriggerEnterHandle += (RoleEntity role, Collider2D other) => {
                OnFootTriggerEnter(ctx, role, other);
            };
            role.OnFootTriggerStayHandle += (RoleEntity role, Collider2D other) => {
                OnFootTriggerStay(ctx, role, other);
            };
            role.OnFootTriggerExitHandle += (RoleEntity role, Collider2D other) => {
                OnFootTriggerExit(role, other);
            };

            role.OnBodyCollisionEnterHandle += (RoleEntity role, Collision2D other) => {
                OnBodyCollisionEnter(ctx, role, other);
            };
            role.OnBodyCollisionStayHandle += (RoleEntity role, Collision2D other) => {
                OnBodyCollisionStay(ctx, role, other);
            };
            role.OnBodyCollisionExitHandle += (RoleEntity role, Collision2D other) => {
                OnBodyCollisionExit(ctx, role, other);
            };

            role.OnBodyTriggerEnterHandle += (RoleEntity role, Collider2D other) => {
                OnBodyTriggerEnter(ctx, role, other);
            };
            ctx.roleRepo.Add(role);
            return role;
        }

        public static void CheckAndUnSpawn(GameBusinessContext ctx, RoleEntity role) {
            if (role.needTearDown) {
                UnSpawn(ctx, role);
            }
        }

        public static void UnSpawn(GameBusinessContext ctx, RoleEntity role) {
            ctx.roleRepo.Remove(role);
            role.TearDown();
        }

        public static void BoxCast(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = Vector2.one;
            var dir = Vector2.down;
            LayerMask layer = (1 << LayConst.GROUND) | (1 << LayConst.BLOCK);

            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.RaycastNonAlloc(pos, dir, hitResults, 1.3f, layer);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK)) {
                    RoleEnterGroundOrBlock(ctx, role);
                } else if (hitGo.CompareTag(TagConst.GROUND)) {
                    RoleEnterGroundOrBlock(ctx, role);
                }
            }
        }

        static void OnFootTriggerEnter(GameBusinessContext ctx, RoleEntity role, Collider2D other) {

            var otherGo = other.gameObject;
            var otherTag = otherGo.tag;

            // Enter Ground Or Block
            if (otherGo.CompareTag(TagConst.SPIKE)) {
                RoleEnterSpike(ctx, role);
            }

        }

        static void OnFootTriggerStay(GameBusinessContext ctx, RoleEntity role, Collider2D other) {
            var otherGo = other.gameObject;
            var otherTag = otherGo.tag;

            // Stay Ground Or Block
            if (otherGo.CompareTag(TagConst.SPIKE)) {
                RoleEnterSpike(ctx, role);
            }

        }

        static void RoleEnterGroundOrBlock(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Ground Or Block
            if (role.Velocity.y <= 0.0001f) {
                role.Move_EnterGround();
            }

            // - Restore Jump & Skill Times
        }

        static void RoleEnterSpike(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Spike
            role.Attr_GetHurt();
        }

        static void OnFootTriggerExit(RoleEntity role, Collider2D other) {
            // Leave Ground Or Block
        }

        static void OnBodyCollisionEnter(GameBusinessContext ctx, RoleEntity role, Collision2D other) {
            // Hurt & Dead
            // Teleport
        }

        static void OnBodyCollisionStay(GameBusinessContext gameContext, RoleEntity role, Collision2D other) {

        }

        static void OnBodyCollisionExit(GameBusinessContext gameContext, RoleEntity role, Collision2D other) {

        }

        static void OnBodyTriggerEnter(GameBusinessContext gameContext, RoleEntity role, Collider2D other) {
            // Eat Star
        }

        public static void ApplyMove(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_ApplyMove(dt);
        }

        public static void ApplyJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Jump();
        }

        public static void ApplyFalling(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Falling(dt);
        }

    }

}