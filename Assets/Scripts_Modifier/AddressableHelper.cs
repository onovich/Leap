#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Leap.Modifier {

    public static class AddressableHelper {

        public static void SetAddressable(UnityEngine.Object asset, string addressableLableName, string addressableGroupName, bool isSetSimplifiedName) {
            AddressableReflection(
                out var Settings_obj, out var groups_obj, out var DefaultGroup_prop, out var DefaultGroup_obj,
                out var CreateOrMoveEntry_met, out var GatherTargetInfos_met, out var SetAaEntry_met
            );

            // 检查组名
            if (!HasGroup(groups_obj, addressableGroupName, out var targetGroup_obj)) {
                targetGroup_obj = DefaultGroup_obj;
            }

            // 设置资源 标签、名称、组
            SetLable(addressableLableName, asset, Settings_obj, targetGroup_obj, CreateOrMoveEntry_met);
            if (isSetSimplifiedName) SetSimplifiedName(asset, Settings_obj, targetGroup_obj, CreateOrMoveEntry_met);
        }

        public static void SaveData(PropertyInfo defaultGroup_prop,
                                   object settings_obj, AddressableAssetGroup targetGroup, MethodInfo gatherTargetInfos_methodInfo,
                                   MethodInfo setAaEntry_methodInfo, object defaultGroup_obj, UnityEngine.Object[] assets) {

            // 临时设置DefaultGroup
            defaultGroup_prop.SetValue(settings_obj, targetGroup);

            // 最终方法调用以生效配置
            object targetInfos_obj = gatherTargetInfos_methodInfo.Invoke(null, new System.Object[] { assets, settings_obj });
            setAaEntry_methodInfo.Invoke(null, new System.Object[] { settings_obj, targetInfos_obj, true });

            // 恢复原DefaultGroup
            defaultGroup_prop.SetValue(settings_obj, defaultGroup_obj);

        }

        public static void AddressableReflection(
              out object Settings_obj, out object groups_obj, out PropertyInfo DefaultGroup_prop, out object DefaultGroup_obj,
              out MethodInfo method_CreateOrMoveEntry, out MethodInfo GatherTargetInfos_methodInfo, out MethodInfo SetAaEntry_methodInfo) {

            Settings_obj = null;
            DefaultGroup_obj = null;
            groups_obj = null;
            DefaultGroup_prop = null;
            method_CreateOrMoveEntry = null;
            GatherTargetInfos_methodInfo = null;
            SetAaEntry_methodInfo = null;

            if (!ReflectionHelper.TryGetCurDomainAssembly("Unity.Addressables.Editor", out var aa_Assembly)) {
                Debug.LogError($"Assembly not found: Unity.Addressables.Editor");
                return;
            }

            // AddressableAssetInspectorGUI
            System.Type AddressableAssetInspectorGUI_type = aa_Assembly.GetType("UnityEditor.AddressableAssets.GUI.AddressableAssetInspectorGUI", true);
            System.Type AddressableAssetSettingsDefaultObject_type = aa_Assembly.GetType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject", true);

            // AddressableAssetSettings
            PropertyInfo Settings_prop = AddressableAssetSettingsDefaultObject_type.GetProperty("Settings");
            System.Type Settings_type = Settings_prop.PropertyType;
            Settings_obj = Settings_prop.GetValue(null);

            // AddressableAssetGroup
            DefaultGroup_prop = Settings_obj.GetType().GetProperty("DefaultGroup");
            DefaultGroup_obj = DefaultGroup_prop.GetValue(Settings_obj);

            /// 获取所有组以此进行组名检查
            PropertyInfo groups_prop = Settings_obj.GetType().GetProperty("groups");
            groups_obj = groups_prop.GetValue(Settings_obj);

            // 相关方法
            method_CreateOrMoveEntry = Settings_type.GetMethod(
               "CreateOrMoveEntry",
               BindingFlags.Public | BindingFlags.Instance
           );
            GatherTargetInfos_methodInfo = AddressableAssetInspectorGUI_type.GetMethod("GatherTargetInfos", BindingFlags.NonPublic | BindingFlags.Static);
            SetAaEntry_methodInfo = AddressableAssetInspectorGUI_type.GetMethod("SetAaEntry", BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void SetSimplifiedName(UnityEngine.Object asset, object Settings_obj, object targetGroup_obj, MethodInfo method_CreateOrMoveEntry) {
            if (asset == null) {
                Debug.LogWarning($"Asset is null!");
                return;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            object assetAAEntry = method_CreateOrMoveEntry.Invoke(Settings_obj, new System.Object[] { guid, targetGroup_obj, null, null });

            // - Name
            MethodInfo method_SetAddress = assetAAEntry.GetType().GetMethod("SetAddress", BindingFlags.Public | BindingFlags.Instance);
            method_SetAddress.Invoke(assetAAEntry, new System.Object[] { asset.name, null });
        }

        public static void SetLabel(UnityEngine.Object asset, object Settings_obj, object targetGroup_obj, MethodInfo method_CreateOrMoveEntry, string label) {
            if (asset == null) {
                Debug.LogWarning($"Asset is null!");
                return;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            object assetAAEntry = method_CreateOrMoveEntry.Invoke(Settings_obj, new System.Object[] { guid, targetGroup_obj, null, null });

            // 清除现有标签
            var existingLabels = assetAAEntry.GetType().GetProperty("labels").GetValue(assetAAEntry) as ICollection<string>;
            if (existingLabels != null) {
                foreach (var existingLabel in existingLabels.ToList()) {
                    MethodInfo method_SetLabel = assetAAEntry.GetType().GetMethod("SetLabel");
                    method_SetLabel.Invoke(assetAAEntry, new System.Object[] { existingLabel, false, null, null });
                }
            }

            // 设置新标签
            MethodInfo method_SetNewLabel = assetAAEntry.GetType().GetMethod("SetLabel");
            method_SetNewLabel.Invoke(assetAAEntry, new System.Object[] { label, true, null, null });

        }

        static void SetLable(string lableName, UnityEngine.Object asset) {
            AddressableReflection(
                out var Settings_obj, out var groups_obj, out var DefaultGroup_prop, out var DefaultGroup_obj,
                out var CreateOrMoveEntry_met, out var GatherTargetInfos_met, out var SetAaEntry_met
            );

            SetLable(lableName, asset, Settings_obj, DefaultGroup_obj, CreateOrMoveEntry_met);
        }

        static void SetLable(string aaLableName, UnityEngine.Object asset, object Settings_obj, object targetGroup_obj, MethodInfo method_CreateOrMoveEntry) {
            if (asset == null) {
                Debug.LogWarning($"AA包资源为空!");
                return;
            }

            var path = AssetDatabase.GetAssetPath(asset);
            var guid = AssetDatabase.AssetPathToGUID(path);
            object assetAAEntry = method_CreateOrMoveEntry.Invoke(Settings_obj, new System.Object[] { guid, targetGroup_obj, null, null });

            // - Label
            if (aaLableName != null) {
                MethodInfo method_SetLable = assetAAEntry.GetType().GetMethod("SetLabel");
                method_SetLable.Invoke(assetAAEntry, new System.Object[] { aaLableName, true, null, null });
            }
        }

        static bool HasGroup(object groups_obj, string groupName, out object tarGroup_obj) {
            tarGroup_obj = null;
            var ie = (IEnumerable<object>)groups_obj;
            foreach (var group in ie) {
                var name = group.GetType().GetProperty("Name").GetValue(group).ToString();
                if (name == groupName) {
                    tarGroup_obj = group;
                    Debug.Log($"找到组:{groupName}");
                    break;
                }
            }

            return tarGroup_obj != null;
        }

    }

}

#endif