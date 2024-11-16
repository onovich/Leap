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

        public static bool Condition_CheckLandGround(GameBusinessContext ctx, RoleEntity role) {
            return role.physics_hitGround;
        }

        public static void Physics_CheckLandGround(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = new Vector2(0.8f, 1f);
            var dir = Vector2.down;
            LayerMask layer = (1 << LayConst.TERRAIN) | (1 << LayConst.BLOCK);
            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0.01f, layer);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK) || hitGo.CompareTag(TagConst.TERRAIN)) {
                    role.physics_hitGround = true;
                    return;
                }
            }
            role.physics_hitGround = false;
        }

        public static bool Condition_CheckHitSpike(GameBusinessContext ctx, RoleEntity role) {
            return role.physics_hitSpike;
        }

        public static void Physics_CheckHitSpike(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = new Vector2(0.8f, 1f);
            var dir = Vector2.down;
            LayerMask layer = 1 << LayConst.SPIKE;
            var hitResults = ctx.hitResults;
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0.3f, layer);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.SPIKE)) {
                    role.physics_hitSpike = true;
                    return;
                }
            }
            role.physics_hitSpike = false;
        }

        public static bool Condition_CheckHitWall(GameBusinessContext ctx, RoleEntity role, out float wallFriction, out Vector2 wallDir) {
            wallFriction = role.physics_wallFriction;
            wallDir = role.physics_hitWallDir;
            return role.physics_hitWall;
        }

        public static void Physics_CheckHitWall(GameBusinessContext ctx, RoleEntity role) {
            var pos = role.Pos;
            var size = role.Size;
            var dir = role.Velocity.normalized;
            role.physics_wallFriction = 0f;
            dir.y = 0;
            LayerMask layer = (1 << LayConst.TERRAIN) | (1 << LayConst.BLOCK);
            var hitResults = ctx.hitResults;
            var config = ctx.templateInfraContext.Config_Get();
            var hitCount = Physics2D.BoxCastNonAlloc(pos, size, 0, dir, hitResults, 0f, layer);
            for (int i = 0; i < hitCount; i++) {
                var hit = hitResults[i];
                var hitGo = hit.collider.gameObject;
                if (hitGo.CompareTag(TagConst.BLOCK)) {
                    role.physics_wallFriction = config.roleBlockFriction;
                    role.physics_hitWall = true;
                    role.physics_hitWallDir = dir;
                    return;
                } else if (hitGo.CompareTag(TagConst.TERRAIN)) {
                    role.physics_wallFriction = config.roleTerrainFriction;
                    role.physics_hitWall = true;
                    role.physics_hitWallDir = dir;
                    return;
                }
            }
            role.physics_hitWallDir = Vector2.zero;
            role.physics_hitWall = false;
        }

        static void GetHurt(GameBusinessContext ctx, RoleEntity role) {
            role.Attr_GetHurt();
        }

        static void OnBodyTriggerEnter(GameBusinessContext gameContext, RoleEntity role, Collider2D other) {
            // Eat Star
        }

        public static void ApplyMove(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_ApplyMove(dt);
        }

        public static bool Condition_InputJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            return role.inputCom.jumpAxis_Temp != 0;
        }

        public static void ApplyJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.Move_Jump();
        }

        public static bool Condition_WallingIsEnd(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.fsmCom.walling_duration -= dt;
            return role.fsmCom.walling_duration <= 0;
        }

        public static bool Condition_WallJumpingIsEnd(GameBusinessContext ctx, RoleEntity role, float dt) {
            role.fsmCom.wallJumping_duration -= dt;
            return role.fsmCom.wallJumping_duration <= 0;
        }

        public static bool Condition_InputHoldingWall(GameBusinessContext ctx, RoleEntity role, Vector2 wallDir, float dt) {
            var succ = role.inputCom.moveAxis.normalized.x == wallDir.normalized.x;
            succ = succ && role.inputCom.moveAxis.normalized.x != 0;
            succ = succ && wallDir.normalized.x != 0;
            return succ;
        }

        public static bool Condition_InputWallJump(GameBusinessContext ctx, RoleEntity role, float dt) {
            return role.inputCom.jumpAxis_Temp != 0;
        }

        public static void ApplyWallJump(GameBusinessContext ctx, RoleEntity role, Vector2 dir) {
            role.Move_WallJump(dir);
        }

        public static void ApplyWallJumpForce(GameBusinessContext ctx, RoleEntity role, Vector2 dir, float dt) {
            role.Move_WallJumpForceTick(dir, dt);
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
            }
            if (rolePos.x < min.x) {
                var diff = min.x - rolePos.x;
                rolePos += new Vector2(diff, 0);
                role.Pos_SetPos(rolePos);
            }
            if (rolePos.x > max.x) {
                var diff = rolePos.x - max.x;
                rolePos -= new Vector2(diff, 0);
                role.Pos_SetPos(rolePos);
            }
            if (rolePos.y < min.y) {
                role.Attr_DeadlyHurt();
            }
        }

        public static void ApplyFalling(GameBusinessContext ctx, RoleEntity role, float friction, float dt) {
            role.Move_Falling(friction, dt);
        }

    }

}