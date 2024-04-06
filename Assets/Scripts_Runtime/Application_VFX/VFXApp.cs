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

        public static void PlayRoleDeadVFX(VFXAppContext ctx, string name, Vector2 pos, float duration) {
            ctx.AddVFXToWorld(name, duration, pos);
        }

    }

}