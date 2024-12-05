using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PrefilterManager : MonoBehaviour
{
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Texture2D environmentMap;
    [SerializeField] private Texture2DArray debugTex;
    [SerializeField] private float mipMapLevel;
    [SerializeField] private int numSamples;
    [SerializeField] private float roughness;
    [SerializeField] private int numSpecularFilterLevels;
    // private RenderTexture filteredMap;

    private int KERNEL_PREFILTER_SPECULAR;
    private int KERNEL_INTEGRATE_BRDF;
    private int KERNEL_GREYSCALE;

    void Start()
    {
        KERNEL_PREFILTER_SPECULAR = computeShader.FindKernel("CS_PrefilterSpecular");
        KERNEL_INTEGRATE_BRDF = computeShader.FindKernel("CS_IntegrateBRDF");
        KERNEL_GREYSCALE = computeShader.FindKernel("CS_Greyscale");

        // int level = 16;

        // filteredMap = new RenderTexture(environmentMap.width / level, environmentMap.height / level, 0)
        // {
        //     enableRandomWrite = true,
        //     graphicsFormat = environmentMap.graphicsFormat,
        //     useMipMap = environmentMap.mipmapCount > 1,
        //     autoGenerateMips = true,
        // };
        // filteredMap.Create();

        // Camera.main.GetComponent<ViewRenderTexture>().renderTexture = filteredMap;

        // computeShader.SetTexture(0, "_EnvironmentMap", environmentMap);
        // computeShader.SetTexture(0, "_FilteredMap", filteredMap);
        // computeShader.SetFloat("_Width", environmentMap.width / level);
        // computeShader.SetFloat("_Height", environmentMap.height / level);
        // computeShader.SetFloat("_MipMapLevel", mipMapLevel);
        // computeShader.SetInt("_Samples", numSamples);
        // computeShader.SetFloat("_Roughness", roughness);

        // computeShader.Dispatch(0, environmentMap.width / level / 8, environmentMap.height / level / 8, 1);

        // // Get the path of the original Texture2D asset
        // string originalTexturePath = AssetDatabase.GetAssetPath(environmentMap);
        // string directoryPath = System.IO.Path.GetDirectoryName(originalTexturePath);

        // // Extract the base file name (without extension) from the original texture path
        // string originalFileName = System.IO.Path.GetFileNameWithoutExtension(originalTexturePath);

        // // Generate the EXR file path by appending "_filtered" to the original file name
        // string exrFileName = originalFileName + "_filtered.exr";
        // string exrFilePath = System.IO.Path.Combine(directoryPath, exrFileName);

        // SaveRenderTextureAsEXR(filteredMap, exrFilePath);

        // GeneratePrefilteredSpecular();
        // IntegrateBRDF();

        GeneratePrefilteredEnvironment();

    }

    void GeneratePrefilteredEnvironment()
    {
        int width = environmentMap.width;
        int height = environmentMap.height;


        RenderTexture filteredMap = new RenderTexture(
            width,
            height,
            0,
            RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear
        );
        filteredMap.filterMode = FilterMode.Bilinear;
        filteredMap.wrapMode = TextureWrapMode.Clamp;
        filteredMap.enableRandomWrite = true;
        filteredMap.useMipMap = false;
        filteredMap.autoGenerateMips = false;
        filteredMap.anisoLevel = 16;
        filteredMap.Create();

        Camera.main.GetComponent<ViewRenderTexture>().renderTexture = filteredMap;

        computeShader.SetFloat("_Width", width);
        computeShader.SetFloat("_Height", height);
        computeShader.SetFloat("_MipMapLevel", mipMapLevel);


        //GreyScale Image
        computeShader.SetTexture(KERNEL_GREYSCALE, "_Target", filteredMap);
        computeShader.SetTexture(KERNEL_GREYSCALE, "_EnvironmentMap", environmentMap);
        computeShader.Dispatch(KERNEL_GREYSCALE, width / 8, height / 8, 1);


    }

    void GeneratePrefilteredSpecular()
    {

        RenderTexture filteredSpecularMap = new RenderTexture(
            environmentMap.width,
            environmentMap.height,
            0,
            RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear
        );
        filteredSpecularMap.dimension = UnityEngine.Rendering.TextureDimension.Tex2DArray;
        filteredSpecularMap.filterMode = FilterMode.Bilinear;
        filteredSpecularMap.wrapMode = TextureWrapMode.Clamp;
        filteredSpecularMap.enableRandomWrite = true;
        filteredSpecularMap.volumeDepth = numSpecularFilterLevels;
        filteredSpecularMap.useMipMap = false;
        filteredSpecularMap.autoGenerateMips = false;
        filteredSpecularMap.anisoLevel = 16;
        filteredSpecularMap.Create();

        computeShader.SetTexture(KERNEL_PREFILTER_SPECULAR, "_EnvironmentMap", environmentMap);
        computeShader.SetTexture(KERNEL_PREFILTER_SPECULAR, "_FilteredSpecularMap", filteredSpecularMap);
        computeShader.SetInt("_NumSpecularFilterLevels", numSpecularFilterLevels);
        computeShader.SetFloat("_Width", environmentMap.width);
        computeShader.SetFloat("_Height", environmentMap.height);
        computeShader.SetInt("_Samples", numSamples);
        computeShader.SetFloat("_MipMapLevel", mipMapLevel);

        computeShader.Dispatch(KERNEL_PREFILTER_SPECULAR, environmentMap.width / 8, environmentMap.height / 8, numSpecularFilterLevels);

        Debug.Log("Done");

        Texture2DArray filteredSpecularArray = ConvertRenderTextureToTexture2DArray(filteredSpecularMap);

        SaveAsAsset(filteredSpecularArray, GetPathWithPostfix(environmentMap, "_filteredSpecular.asset"));

        filteredSpecularMap.Release();
        Destroy(filteredSpecularMap);
    }

    void IntegrateBRDF()
    {
        const int size = 512;
        RenderTexture integratedBRDF = new RenderTexture(
            size,
            size,
            0,
            RenderTextureFormat.ARGBFloat,
            RenderTextureReadWrite.Linear
        );
        integratedBRDF.filterMode = FilterMode.Bilinear;
        integratedBRDF.wrapMode = TextureWrapMode.Clamp;
        integratedBRDF.enableRandomWrite = true;
        integratedBRDF.useMipMap = false;
        integratedBRDF.autoGenerateMips = false;
        integratedBRDF.anisoLevel = 16;
        integratedBRDF.Create();

        computeShader.SetTexture(KERNEL_INTEGRATE_BRDF, "_IntegratedBRDF", integratedBRDF);
        computeShader.SetFloat("_Width", size);
        computeShader.SetFloat("_Height", size);
        computeShader.SetInt("_Samples", numSamples);

        computeShader.Dispatch(KERNEL_INTEGRATE_BRDF, size / 8, size / 8, 1);

        SaveAsAsset(ConvertRenderTextureToTexture2D(integratedBRDF), "Assets/Resources/IntegrationMaps/GGXBrdfIntegration.asset");

        integratedBRDF.Release();
        Destroy(integratedBRDF);

    }

    Texture2DArray ConvertRenderTextureToTexture2DArray(RenderTexture rt)
    {
        Texture2DArray texture2DArray = new Texture2DArray(
            width: rt.width,
            height: rt.height,
            depth: rt.volumeDepth,
            format: UnityEngine.Experimental.Rendering.DefaultFormat.HDR,
            UnityEngine.Experimental.Rendering.TextureCreationFlags.None
        );

        // Temporary RenderTexture for reading each slice
        RenderTexture tempRT = RenderTexture.GetTemporary(
            rt.width,
            rt.height,
            0,
            rt.graphicsFormat
        );

        for (int slice = 0; slice < rt.volumeDepth; slice++)
        {
            // Copy the specific slice to a temporary RenderTexture
            Graphics.CopyTexture(rt, slice, 0, tempRT, 0, 0);

            // Create a temporary Texture2D to read the pixels
            Texture2D tempTex2D = new Texture2D(
                width: rt.width,
                height: rt.height,
                format: UnityEngine.Experimental.Rendering.DefaultFormat.HDR,
                UnityEngine.Experimental.Rendering.TextureCreationFlags.None
            );

            // Read pixels from the temporary RenderTexture
            RenderTexture.active = tempRT;
            tempTex2D.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
            tempTex2D.Apply();

            // Copy pixels to the Texture2DArray
            texture2DArray.SetPixels(tempTex2D.GetPixels(), slice);

            // Clean up temporary Texture2D
            Destroy(tempTex2D);
        }

        // Clean up
        RenderTexture.ReleaseTemporary(tempRT);
        texture2DArray.Apply();

        return texture2DArray;
    }

    Texture2D ConvertRenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D texture = new Texture2D(
            renderTexture.width,
            renderTexture.height,
            TextureFormat.RGBAFloat,
            false,
            true
        );

        // Copy the RenderTexture content to the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

        return texture;
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
        byte[] exrData = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat); // CompressZIP for lossless compression
        System.IO.File.WriteAllBytes(filePath, exrData);

        Debug.Log("Saved EXR file to " + filePath);
    }

    void SaveTextureAsEXR(Texture2D texture, string filePath)
    {
        byte[] exrData = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
        System.IO.File.WriteAllBytes(filePath, exrData);

        Debug.Log("Saved EXR file to " + filePath);
    }

    // Save Texture2DArray as an asset
    void SaveAsAsset(Object asset, string assetPath)
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(asset, assetPath);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log("Saved Asset to " + assetPath);
#endif
    }

    string GetPathWithPostfix(Object obj, string postfix)
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

}
