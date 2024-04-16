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

        public static void BoxCast(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = new Vector2(0.8f, 1f);
            var dir = Vector2.down;
            LayerMask layer = (1 << LayConst.GROUND) | (1 << LayConst.BLOCK) | (1 << LayConst.SPIKE);

            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0.3f, layer);
            Debug.DrawRay(pos, dir * 0.8f, Color.red);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK)) {
                    OnFootEnterGroundOrBlock(ctx, role);
                } else if (hitGo.CompareTag(TagConst.GROUND)) {
                    OnFootEnterGroundOrBlock(ctx, role);
                } else if (hitGo.CompareTag(TagConst.SPIKE)) {
                    OnFootEnterSpike(ctx, role);
                }
            }
        }

        static void OnFootEnterGroundOrBlock(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Ground Or Block
            if (role.Velocity.y <= 0.0001f) {
                role.Move_EnterGround();
            }

            // - Restore Jump & Skill Times
        }

        static void OnBodyCollisionEnterWall(GameBusinessContext ctx, RoleEntity role, Collision2D coll) {
            // - Enter Wall
            if (!coll.transform.CompareTag(TagConst.BLOCK) && !coll.transform.CompareTag(TagConst.GROUND)) {
                return;
            }
            if (role.isGround) {
                return;
            }
            role.Move_EnterWall();

            GLog.Log("Enter Wall");
        }

        static void OnBodyCollisionExitWall(GameBusinessContext ctx, RoleEntity role, Collision2D coll) {
            // - Exit Wall
            if (!coll.transform.CompareTag(TagConst.BLOCK) && !coll.transform.CompareTag(TagConst.GROUND)) {
                return;
            }
            role.Move_LeaveWall();

            GLog.Log("Exit Wall");
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

        public static void ApplyWallJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_WallJump();
        }

        public static void ApplyFalling(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Falling(dt);
        }

    }

}