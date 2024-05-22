#if UNITY_EDITOR

using System.Reflection;

namespace Leap.Modifier {

    public static class ReflectionHelper {

        public static bool TryGetCurDomainAssembly(string assemblyName, out Assembly assembly) {

            var domain = System.AppDomain.CurrentDomain;
            var allAssembly = domain.GetAssemblies();
            assembly = null;
            foreach (var asm in allAssembly) {
                if (asm.GetName().Name == assemblyName) {
                    assembly = asm;
                    break;
                }
            }
            return assembly != null;

        }

    }

}

#endif