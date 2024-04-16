using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Leap.Modifier {

    public static class FieldHelper {

        public static List<T> GetAllInstances<T>() where T : ScriptableObject {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            List<T> assets = new List<T>();

            foreach (string guid in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    assets.Add(asset);
                }
            }

            return assets;
        }

    }

}