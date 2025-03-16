
using UnityEditor;
using UnityEngine;

public static class Saver
{
#if UNITY_EDITOR
    public static void SaveRenderTextureAsEXR(RenderTexture renderTexture, string filePath)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);

        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        byte[] exrData = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat); // CompressZIP for lossless compression
        System.IO.File.WriteAllBytes(filePath, exrData);

        Debug.Log("Saved EXR file to " + filePath);
    }

    public static void SaveTextureAsEXR(Texture2D texture, string filePath)
    {
        byte[] exrData = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
        System.IO.File.WriteAllBytes(filePath, exrData);

        Debug.Log("Saved EXR file to " + filePath);
    }

    public static void SaveAsAsset(Object asset, string assetPath)
    {
        UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Saved Asset to " + assetPath);
    }

    public static string GetPathWithPostfix(Object obj, string postfix)
    {
        // Get the path of the original Texture2D asset
        string originalTexturePath = AssetDatabase.GetAssetPath(obj);
        string directoryPath = System.IO.Path.GetDirectoryName(originalTexturePath);

        // Extract the base file name (without extension) from the original texture path
        string originalFileName = System.IO.Path.GetFileNameWithoutExtension(originalTexturePath);

        string fileName = originalFileName + postfix;
        string filePath = System.IO.Path.Combine(directoryPath, fileName);

        return filePath;
    }

    public static Texture2D SaveScreenShotAsAsset(string path)
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        Camera.main.targetTexture = null;

        SaveAsAsset(texture, path);

        return texture;
    }
#endif
}