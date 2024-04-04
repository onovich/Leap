using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap {

    public class RoleRepository {

        Dictionary<int, RoleEntity> all;
        Dictionary<Vector2Int, List<RoleEntity>> posDict;

        RoleEntity[] temp;
        Vector2Int[] cellsTemp;

        public RoleRepository() {
            all = new Dictionary<int, RoleEntity>();
            posDict = new Dictionary<Vector2Int, List<RoleEntity>>();
            temp = new RoleEntity[1000];
            cellsTemp = new Vector2Int[1000];
        }

        public void Add(RoleEntity role) {
            all.Add(role.entityID, role);
            var pos = role.Pos_GetPosInt();
            if (posDict.TryGetValue(pos, out var roles)) {
                roles.Add(role);
            } else {
                var list = new List<RoleEntity>();
                list.Add(role);
                posDict.Add(pos, list);
            }
        }

        public int TakeAll(out RoleEntity[] roles) {
            int count = all.Count;
            if (count > temp.Length) {
                temp = new RoleEntity[(int)(count * 1.5f)];
            }
            all.Values.CopyTo(temp, 0);
            roles = temp;
            return count;
        }

        public void Remove(RoleEntity role) {
            all.Remove(role.entityID);
            
            var pos = role.Pos_GetPosInt();
            if (posDict.ContainsKey(pos)) {
                posDict[pos].Remove(role);
            }
        }

        public void UpdatePosDict(RoleEntity role) {
            var lastPos = role.Pos_GetLastPosInt();
            var newPos = role.Pos_GetPosInt();
            if (posDict.ContainsKey(lastPos)) {
                posDict[lastPos].Remove(role);
            } else {
                GLog.LogError("Dont's has old key in PosDict, role entityID = " + role.entityID);
            }
            if (posDict.ContainsKey(newPos)) {
                posDict[newPos].Add(role);
            } else {
                var list = new List<RoleEntity>();
                list.Add(role);
                posDict.Add(newPos, list);
            }
        }

        public bool TryGetRoleByPosInt(Vector2Int pos, out List<RoleEntity> roles) {
            if (!posDict.TryGetValue(pos, out roles)) {
                return false;
            }
            return true;
        }

        public int TryGetAround(Vector2Int centerPos, float radius, int maxCount, out RoleEntity[] roles) {
            int gridCount = GameFunctions.GFGrid.CircleCycle_GetCells(centerPos, radius, cellsTemp);
            int roleCount = 0;
            for (int i = 0; i < gridCount; i++) {
                if (!posDict.TryGetValue(cellsTemp[i], out var list)) {
                    continue;
                }
                list.ForEach((role) => {
                    temp[roleCount++] = role;
                    if (roleCount >= maxCount) {
                        return;
                    }
                });
                if (roleCount >= maxCount) {
                    break;
                }
            }
            roles = temp;
            return roleCount;
        }

        public int TryGetAroundWithAlly(int entityID, AllyStatus allyStatus, Vector2Int centerPos, float radius, int maxCount, out RoleEntity[] roles) {
            int gridCount = GameFunctions.GFGrid.CircleCycle_GetCells(centerPos, radius, cellsTemp);
            int roleCount = 0;
            for (int i = 0; i < gridCount; i++) {
                if (!posDict.TryGetValue(cellsTemp[i], out var list)) {
                    continue;
                }
                list.ForEach((role) => {
                    if (role.allyStatus != allyStatus) {
                        return;
                    }
                    if (role.entityID == entityID) {
                        return;
                    }
                    temp[roleCount++] = role;
                    if (roleCount >= maxCount) {
                        return;
                    }
                });
                if (roleCount >= maxCount) {
                    break;
                }
            }
            roles = temp;
            return roleCount;
        }

        public bool TryGetRole(int entityID, out RoleEntity role) {
            return all.TryGetValue(entityID, out role);
        }

        public bool IsInRange(int entityID, in Vector2 pos, float range) {
            bool has = TryGetRole(entityID, out var role);
            if (!has) {
                return false;
            }
            return Vector2.SqrMagnitude(role.Pos_GetPos() - pos) <= range * range;
        }

        public void ForEach(Action<RoleEntity> action) {
            foreach (var role in all.Values) {
                action(role);
            }
        }

        public RoleEntity GetNeareast(AllyStatus allyStatus, Vector2 pos, float radius) {
            RoleEntity nearestRole = null;
            float nearestDist = float.MaxValue;
            float radiusSqr = radius * radius;
            foreach (var role in all.Values) {
                if (role.allyStatus != allyStatus) {
                    continue;
                }
                float dist = Vector2.SqrMagnitude(role.Pos_GetPos() - pos);
                if (dist <= radiusSqr && dist < nearestDist) {
                    nearestDist = dist;
                    nearestRole = role;
                }
            }
            return nearestRole;
        }

        public void Clear() {
            all.Clear();
            posDict.Clear();
        }

    }

}