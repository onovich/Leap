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

            role.Move_LeaveGround();
            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0.3f, layer);
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

        public static void Tick_BoxCastWall(GameBusinessContext ctx, RoleEntity role, float dt) {
            var pos = role.Pos;
            var size = role.Size;
            var dir = role.Velocity.normalized;
            dir.y = 0;
            LayerMask layer = (1 << LayConst.TERRAIN) | (1 << LayConst.BLOCK);

            role.Move_LeaveWall();
            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0f, layer);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK) || hitGo.CompareTag(TagConst.TERRAIN)) {
                    OnBodyEnterWall(ctx, role, hit.collider, hit.normal);
                }
            }

            role.Move_ResetEnterWallDir_Tick(dt);
        }

        static void OnFootEnterGround(GameBusinessContext ctx, RoleEntity role) {
            // - Enter Ground Or Block
            if (role.Velocity.y <= 0.0001f) {
                role.Move_EnterGround();
            }

            // - Restore Jump & Skill Times
        }

        static void OnBodyEnterWall(GameBusinessContext ctx, RoleEntity role, Collider2D coll, Vector2 normal) {
            // - Enter Wall
            if (role.isGround) {
                return;
            }
            var horizontalDir = new Vector2(-normal.x, 0);
            var config = ctx.templateInfraContext.Config_Get();
            if (coll.transform.CompareTag(TagConst.BLOCK)) {
                var friction = config.roleBlockFriction;
                role.Move_EnterWall(horizontalDir, friction);
            }
            if (coll.transform.CompareTag(TagConst.TERRAIN)) {
                var friction = config.roleTerrainFriction;
                role.Move_EnterWall(horizontalDir, friction);
            }
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

        public static bool Condition_Jump(GameBusinessContext ctx, RoleEntity role, float dt) {
            return role.isGround && role.inputCom.jumpAxis_Temp != 0;
        }

        public static void ApplyJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Jump();
        }

        public static void ApplyHitWall(GameBusinessContext ctx, RoleEntity role, float dt) {
            if (role.isWall && role.enterWallDir == role.fsmCom.wallJumping_jumpingDir || role.isGround) {
                role.fsmCom.EnterWalking();
            }
        }

        public static bool Condition_WallJumpIsEnd(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.fsmCom.wallJumping_timer -= dt;
            return role.fsmCom.wallJumping_timer <= 0;
        }

        public static bool Condition_WallJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            return role.isWall && role.inputCom.jumpAxis_Temp != 0 && role.enterWallDir.x != 0;
        }

        public static void ApplyWallJump(GameBusinessContext ctx, RoleEntity role, Vector2 dir) {
            role.Move_WallJump(dir);
        }

        public static void ApplyConstraint(GameBusinessContext ctx, RoleEntity role, float dt) {
            var map = ctx.currentMapEntity;
            var size = map.constraintSize;
            var center = map.constraintCenter;

            var rolePos = role.Pos;
            var roleSize = role.Size;

            var min = center - size / 2;
            var max = center + size / 2 - roleSize;

            if (rolePos.y > max.y) {
                var diff = rolePos.y - max.y;
                rolePos -= new Vector2(0, diff);
                role.Pos_SetPos(rolePos);
                role.fsmCom.EnterWalking();
            }
            if (rolePos.x < min.x) {
                var diff = min.x - rolePos.x;
                rolePos += new Vector2(diff, 0);
                role.Pos_SetPos(rolePos);
                role.fsmCom.EnterWalking();
            }
            if (rolePos.x > max.x) {
                var diff = rolePos.x - max.x;
                rolePos -= new Vector2(diff, 0);
                role.Pos_SetPos(rolePos);
                role.fsmCom.EnterWalking();
            }
            if (rolePos.y < min.y) {
                role.Attr_DeadlyHurt();
            }
        }

        public static void ApplyFalling(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Falling(dt);
        }

        public static bool Condition_IsGround(GameBusinessContext ctx, RoleEntity role, float dt) {
            return role.isGround;
        }

    }

}