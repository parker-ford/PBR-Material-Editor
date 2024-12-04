using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PrefilterManager : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Texture2D environmentMap;
    [SerializeField] private float mipMapLevel;
    [SerializeField] private int numSamples;
    private RenderTexture filteredMap;
    void Start()
    {

        filteredMap = new RenderTexture(environmentMap.width, environmentMap.height, 0)
        {
            enableRandomWrite = true,
            graphicsFormat = environmentMap.graphicsFormat,
            useMipMap = environmentMap.mipmapCount > 1,
            autoGenerateMips = true,
        };
        filteredMap.Create();

        Camera.main.GetComponent<ViewRenderTexture>().renderTexture = filteredMap;

        computeShader.SetTexture(0, "_EnvironmentMap", environmentMap);
        computeShader.SetTexture(0, "_FilteredMap", filteredMap);
        computeShader.SetFloat("_Width", environmentMap.width);
        computeShader.SetFloat("_Height", environmentMap.height);
        computeShader.SetFloat("_MipMapLevel", mipMapLevel);
        computeShader.SetInt("_Samples", numSamples);

        computeShader.Dispatch(0, environmentMap.width / 8, environmentMap.height / 8, 1);

        // Get the path of the original Texture2D asset
        string originalTexturePath = AssetDatabase.GetAssetPath(environmentMap);
        string directoryPath = System.IO.Path.GetDirectoryName(originalTexturePath);

        // Extract the base file name (without extension) from the original texture path
        string originalFileName = System.IO.Path.GetFileNameWithoutExtension(originalTexturePath);

        // Generate the EXR file path by appending "_filtered" to the original file name
        string exrFileName = originalFileName + "_filtered.exr";
        string exrFilePath = System.IO.Path.Combine(directoryPath, exrFileName);

        SaveRenderTextureAsEXR(filteredMap, exrFilePath);

    }

    void SaveRenderTextureAsEXR(RenderTexture renderTexture, string filePath)
    {
        // Convert RenderTexture to Texture2D
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);

        // Copy the RenderTexture content to the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // Save as EXR
        byte[] exrData = texture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP); // CompressZIP for lossless compression
        System.IO.File.WriteAllBytes(filePath, exrData);

        Debug.Log("Saved EXR file to " + filePath);
    }

    void SaveRenderTextureAsPFM(RenderTexture renderTexture, string filePath)
    {
        // Create a temporary Texture2D to hold the RenderTexture data
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);

        // Copy the RenderTexture to the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        // Convert the Texture2D to PFM format and save it as a .pfm file
        byte[] pfmBytes = ConvertToPFM(texture);
        File.WriteAllBytes(filePath, pfmBytes);

        Debug.Log("Saved PFM file to " + filePath);
    }

    byte[] ConvertToPFM(Texture2D texture)
    {
        // For simplicity, assuming the texture uses RGBA32 floating point format
        Color[] pixels = texture.GetPixels();

        // PFM Header
        StringBuilder header = new StringBuilder();
        header.AppendLine("Pf"); // Magic string for PFM (for grayscale images, use "Pf", for color "PF")
        header.AppendLine($"{texture.width} {texture.height}"); // Image dimensions
        header.AppendLine("-1.0"); // The '-1.0' value indicates the float data is stored in little-endian format

        // Prepare the byte data for the image
        using (MemoryStream ms = new MemoryStream())
        {
            // Write the header
            byte[] headerBytes = Encoding.ASCII.GetBytes(header.ToString());
            ms.Write(headerBytes, 0, headerBytes.Length);

            // Write the pixel data in little-endian order (4 bytes per float)
            foreach (Color pixel in pixels)
            {
                WriteFloat(ms, pixel.r); // Red channel
                WriteFloat(ms, pixel.g); // Green channel
                WriteFloat(ms, pixel.b); // Blue channel
                WriteFloat(ms, pixel.a); // Alpha channel (optional, if needed)
            }

            return ms.ToArray();
        }
    }

    void WriteFloat(MemoryStream ms, float value)
    {
        // Convert the float to bytes and write to the memory stream in little-endian order
        byte[] bytes = System.BitConverter.GetBytes(value);
        ms.Write(bytes, 0, bytes.Length);
    }
}
