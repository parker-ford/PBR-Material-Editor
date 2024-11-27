using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Loader
{
    public static List<T> LoadScriptableObjects<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        List<T> results = new List<T>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                results.Add(asset);
            }
        }

        return results;
    }

    public static Texture2D LoadImage(string imagePath)
    {
        return AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath);
    }
}
