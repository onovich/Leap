using UnityEngine;

namespace Leap {

    public static class GameRoleDomain {

        public static RoleEntity Spawn(GameBusinessContext ctx, int typeID, Vector2 pos) {
            var role = GameFactory.Role_Spawn(ctx.templateInfraContext,
                                              ctx.assetsInfraContext,
                                              ctx.idRecordService,
                                              typeID,
                                              pos);
            role.OnBodyTriggerEnterHandle += (RoleEntity role, Collider2D other) => {
                OnBodyTriggerEnter(ctx, role, other);
            };
            role.OnBodyCollisionEnterHandle += (RoleEntity role, Collision2D coll) => {
                OnBodyCollisionEnterWall(ctx, role, coll);
            };
            role.OnBodyCollisionExitHandle += (RoleEntity role, Collision2D coll) => {
                OnBodyCollisionExitWall(ctx, role, coll);
            };
            // role.OnBodyCollisionStayHandle += (RoleEntity role, Collision2D coll) => {
            //     OnBodyCollisionEnterWall(ctx, role, coll);
            // };
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

        public static void BoxCastGround(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = new Vector2(0.8f, 1f);
            var dir = Vector2.down;
            LayerMask layer = (1 << LayConst.TERRAIN) | (1 << LayConst.BLOCK) | (1 << LayConst.SPIKE);

            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0.3f, layer);
            Debug.DrawRay(pos, dir * 0.8f, Color.red);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK) || hitGo.CompareTag(TagConst.TERRAIN)) {
                    OnFootEnterGround(ctx, role);
                } else if (hitGo.CompareTag(TagConst.SPIKE)) {
                    OnFootEnterSpike(ctx, role);
                }
            }
        }

        static void OnFootEnterGround(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Ground Or Block
            if (role.Velocity.y <= 0.0001f) {
                role.Move_EnterGround();
            }

            // - Restore Jump & Skill Times
        }

        static void OnBodyCollisionEnterWall(GameBusinessContext ctx, RoleEntity role, Collision2D coll) {
            // - Enter Wall
            if (role.isGround) {
                return;
            }
            if (coll.transform.CompareTag(TagConst.BLOCK)) {
                var normal = coll.contacts[0].normal;
                var go = coll.transform.parent.parent;
                var entity = go.GetComponent<BlockEntity>();
                if (entity == null) {
                    GLog.LogError($"Didn't Find BlockEntity At: {entity.gameObject.name}");
                }
                var friction = GameBlockDomain.GetFallingFriction(ctx, entity.typeID);
                role.Move_EnterWall(-normal, friction);
            }
            if (coll.transform.CompareTag(TagConst.TERRAIN)) {
                var point = coll.collider.ClosestPoint(role.Pos);
                var dir = point - role.Pos;
                var normal = coll.contacts[0].normal;
                var pos = role.Pos + dir.normalized;
                var friction = GameMapDomain.Terrain_GetFallingFriction(ctx, ctx.currentMapEntity, pos, dir);
                role.Move_EnterWall(-normal, friction);
            }
        }

        static void OnBodyCollisionExitWall(GameBusinessContext ctx, RoleEntity role, Collision2D coll) {
            // - Exit Wall
            if (!coll.transform.CompareTag(TagConst.BLOCK) && !coll.transform.CompareTag(TagConst.TERRAIN)) {
                return;
            }
            role.Move_LeaveWall();
        }

        static void OnFootEnterSpike(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Spike
            role.Attr_GetHurt();
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

        public static void ApplyHoldWall(GameBusinessContext ctx, RoleEntity role, float dt) {
            if (role.Move_TryHoldWall()) {
                role.Move_HoldWall();
            } else {
                role.Move_LeaveHoldWall();
            }
        }

        public static void ApplyWallJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            if (!role.isHoldWall) {
                return;
            }
            role.Move_WallJump();
        }

        public static void ApplyFalling(GameBusinessContext ctx, RoleEntity role, float dt) {
            var fallingFriction = 0f;
            if (role.isHoldWall) {
                fallingFriction = role.holdWallFriction;
                role.Move_Falling(dt, fallingFriction);
            }

            role.Move_Falling(dt, fallingFriction);
        }

    }

}