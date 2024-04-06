using System;
using System.Threading.Tasks;
using Leap.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Leap {
    public static class VFXApp {

        public static void Init(VFXAppContext ctx) {

        }

        public static async Task LoadAssets(VFXAppContext ctx) {
            try {
                await ctx.vfxCore.LoadAssets();
            } catch (Exception e) {
                GLog.LogError(e.ToString());
            }
        }

        public static void LateTick(VFXAppContext ctx, float dt) {
            ctx.vfxCore.Tick(dt);
        }

        public static void AddVFXToWorld(VFXAppContext ctx, string vfxName, float duration, Vector2 pos) {
            ctx.vfxCore.TrySpawnAndPlayVFX_ToWorldPos(vfxName, duration, pos);
        }

        public static void AddVFXToTarget(VFXAppContext ctx, string vfxName, float duration, Transform target) {
            ctx.vfxCore.TrySpawnAndPlayVFX_ToTarget(vfxName, duration, target, Vector3.zero);
        }

        public static void PlayVFXManualy(VFXAppContext ctx, int preSpawnVFXID) {
            ctx.vfxCore.TryPlayManualy(preSpawnVFXID);
        }

        public static void StopVFXManualy(VFXAppContext ctx, int preSpawnVFXID) {
            ctx.vfxCore.TryStopManualy(preSpawnVFXID);
        }

        public static void TearDown(VFXAppContext ctx) {
            ctx.vfxCore.TearDown();
        }

    }

}