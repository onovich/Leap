using System;
using System.Collections.Generic;
using UnityEngine;

namespace Leap {

    public class SpikeRepository {

        Dictionary<int, SpikeEntity> all;
        SpikeEntity[] temp;

        public SpikeRepository() {
            all = new Dictionary<int, SpikeEntity>();
            temp = new SpikeEntity[1000];
        }

        public void Add(SpikeEntity spike) {
            all.Add(spike.entityID, spike);
        }

        public int TakeAll(out SpikeEntity[] spikes) {
            int count = all.Count;
            if (count > temp.Length) {
                temp = new SpikeEntity[(int)(count * 1.5f)];
            }
            all.Values.CopyTo(temp, 0);
            spikes = temp;
            return count;
        }

        public void Remove(SpikeEntity spike) {
            all.Remove(spike.entityID);
        }

        public bool TryGetSpike(int entityID, out SpikeEntity spike) {
            return all.TryGetValue(entityID, out spike);
        }

        public bool IsInRange(int entityID, in Vector2 pos, float range) {
            bool has = TryGetSpike(entityID, out var spike);
            if (!has) {
                return false;
            }
            return Vector2.SqrMagnitude(spike.Pos - pos) <= range * range;
        }

        public void ForEach(Action<SpikeEntity> action) {
            foreach (var spike in all.Values) {
                action(spike);
            }
        }

        public SpikeEntity GetNeareast(Vector2 pos, float radius) {
            SpikeEntity nearestSpike = null;
            float nearestDist = float.MaxValue;
            float radiusSqr = radius * radius;
            foreach (var spike in all.Values) {
                float dist = Vector2.SqrMagnitude(spike.Pos - pos);
                if (dist <= radiusSqr && dist < nearestDist) {
                    nearestDist = dist;
                    nearestSpike = spike;
                }
            }
            return nearestSpike;
        }

        public void Clear() {
            all.Clear();
        }

    }

}