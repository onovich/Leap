using System;
using System.Threading.Tasks;
using TenonKit.Prism;
using UnityEngine;

namespace Leap {

    public class VFXAppContext {

        // Core
        public VFXCore vfxCore;

        public VFXAppContext(string label, Transform root) {
            vfxCore = new VFXCore(label, root);
        }

        // Load
        public async Task LoadAssets() {
            try {
                await vfxCore.LoadAssets();
            } catch (Exception e) {
                GLog.Log(e.ToString());
            }
        }

        // Tick
        public void AddVFXToWorld(string vfxName, float duration, Vector2 pos) {
            vfxCore.TrySpawnAndPlayVFX_ToWorldPos(vfxName, duration, pos);
        }

        public void AddVFXToTarget(string vfxName, float duration, Transform target) {
            vfxCore.TrySpawnAndPlayVFX_ToTarget(vfxName, duration, target, Vector3.zero);
        }

        public void PlayVFXManualy(int preSpawnVFXID) {
            vfxCore.TryPlayManualy(preSpawnVFXID);
        }

        public void StopVFXManualy(int preSpawnVFXID) {
            vfxCore.TryStopManualy(preSpawnVFXID);
        }

        public void TearDown(VFXAppContext ctx) {
            ctx.vfxCore.TearDown();
        }

    }

}