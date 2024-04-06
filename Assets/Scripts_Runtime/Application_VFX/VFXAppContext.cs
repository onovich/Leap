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

    }

}