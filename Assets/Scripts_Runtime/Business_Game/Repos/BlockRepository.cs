using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap {

    public class BlockRepository {

        Dictionary<int, BlockEntity> all;
        BlockEntity[] temp;

        public BlockRepository() {
            all = new Dictionary<int, BlockEntity>();
            temp = new BlockEntity[1000];
        }

        public void Add(BlockEntity block) {
            all.Add(block.entityIndex, block);
        }

        public int TakeAll(out BlockEntity[] blocks) {
            int count = all.Count;
            if (count > temp.Length) {
                temp = new BlockEntity[(int)(count * 1.5f)];
            }
            all.Values.CopyTo(temp, 0);
            blocks = temp;
            return count;
        }

        public void Remove(BlockEntity block) {
            all.Remove(block.entityIndex);
        }

        public bool TryGetBlock(int entityID, out BlockEntity block) {
            return all.TryGetValue(entityID, out block);
        }

        public bool IsInRange(int entityID, in Vector2 pos, float range) {
            bool has = TryGetBlock(entityID, out var block);
            if (!has) {
                return false;
            }
            return Vector2.SqrMagnitude(block.Pos - pos) <= range * range;
        }

        public void ForEach(Action<BlockEntity> action) {
            foreach (var block in all.Values) {
                action(block);
            }
        }

        public BlockEntity GetNeareast(Vector2 pos, float radius) {
            BlockEntity nearestBlock = null;
            float nearestDist = float.MaxValue;
            float radiusSqr = radius * radius;
            foreach (var block in all.Values) {
                float dist = Vector2.SqrMagnitude(block.Pos - pos);
                if (dist <= radiusSqr && dist < nearestDist) {
                    nearestDist = dist;
                    nearestBlock = block;
                }
            }
            return nearestBlock;
        }

        public void Clear() {
            all.Clear();
        }

    }

}