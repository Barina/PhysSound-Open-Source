using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace PhysSound
{
    public class CreatePhysSoundMaterial
    {
        [MenuItem("Assets/Create/PhysSound Material")]
        public static void Create()
        {
            PhysSoundMaterial asset = ScriptableObject.CreateInstance<PhysSoundMaterial>();

            AssetDatabase.CreateAsset(asset, GetActiveFolderName() + "/PhysSoundMaterial.asset");
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = asset;
        }

        private static string GetActiveFolderName()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath.Invoke(null, new object[0]);
            return obj.ToString();
        }
    }
}