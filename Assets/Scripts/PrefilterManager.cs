using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PrefilterManager : MonoBehaviour
{
    [SerializeField] private string objectID;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private Texture2D environmentMap;
    [SerializeField] private float mipMapLevel;
    [SerializeField] private int numSamples;
    [SerializeField] private int numSpecularFilterLevels;

    private int KERNEL_PREFILTER_SPECULAR;
    private int KERNEL_INTEGRATE_BRDF;
    private int KERNEL_GREYSCALE;
    private int KERNEL_AVERAGE_HORIZONTAL;
    private int KERNEL_AVERAGE_VERTICAL;
    private int KERNEL_DIVIDE;
    private int KERNEL_CDF_MARGINAL_INVERSE;
    private int KERNEL_CDF_CONDITIONAL_INVERSE;
    private int KERNEL_PREFILTER_DIFFUSE;

    void Start()
    {
        KERNEL_PREFILTER_SPECULAR = computeShader.FindKernel("CS_PrefilterSpecular");
        KERNEL_INTEGRATE_BRDF = computeShader.FindKernel("CS_IntegrateBRDF");
        KERNEL_GREYSCALE = computeShader.FindKernel("CS_Greyscale");
        KERNEL_AVERAGE_HORIZONTAL = computeShader.FindKernel("CS_AverageHorizontal");
        KERNEL_AVERAGE_VERTICAL = computeShader.FindKernel("CS_AverageVertical");
        KERNEL_DIVIDE = computeShader.FindKernel("CS_Divide");
        KERNEL_CDF_MARGINAL_INVERSE = computeShader.FindKernel("CS_CDFMarginalInverse");
        KERNEL_CDF_CONDITIONAL_INVERSE = computeShader.FindKernel("CS_CDFConditionalInverse");
        KERNEL_PREFILTER_DIFFUSE = computeShader.FindKernel("CS_PrefilterEnvironment");

        Material skybox = new Material(Shader.Find("Skybox/Panoramic"));
        skybox.SetTexture("_MainTex", environmentMap);

        Texture2D filteredDiffuse = GeneratePrefilteredDiffuse();
        Texture2DArray filteredSpecular = GeneratePrefilteredSpecular();

        SaveAsAsset(skybox, GetPathWithPostfix(environmentMap, "_skybox.asset"));
        SaveAsAsset(filteredDiffuse, GetPathWithPostfix(environmentMap, "_filteredDiffuse.asset"));
        SaveAsAsset(filteredSpecular, GetPathWithPostfix(environmentMap, "_filteredSpecular.asset"));

        EnvironmentObject obj = new EnvironmentObject();

        obj.id = objectID;
        obj.displayImage = environmentMap;
        obj.skyboxMaterial = skybox;
        obj.filteredDiffuseMap = filteredDiffuse;
        obj.filteredSpecularMap = filteredSpecular;

        SaveAsAsset(obj, "Assets/EnvironmentObject/" + objectID + ".asset");

        Debug.Log("Done Saving " + objectID);

    }
    Texture2D GeneratePrefilteredDiffuse()
    {
        int width = environmentMap.width;
        int height = environmentMap.height;

        computeShader.SetFloat("_Width", width);
        computeShader.SetFloat("_Height", height);
        computeShader.SetFloat("_MipMapLevel", mipMapLevel);
        computeShader.SetInt("_Samples", numSamples);

        //GreyScale Image
        RenderTexture greyScale = CreateRenderTexture(width, height);
        computeShader.SetTexture(KERNEL_GREYSCALE, "_Target", greyScale);
        computeShader.SetTexture(KERNEL_GREYSCALE, "_EnvironmentMap", environmentMap);
        computeShader.Dispatch(KERNEL_GREYSCALE, width / 8, height / 8, 1);

        //Average Horizontal
        RenderTexture horizontalAverage = CreateRenderTexture(1, height);
        computeShader.SetTexture(KERNEL_AVERAGE_HORIZONTAL, "_Target", horizontalAverage);
        computeShader.SetTexture(KERNEL_AVERAGE_HORIZONTAL, "_EnvironmentMap", greyScale);
        computeShader.Dispatch(KERNEL_AVERAGE_HORIZONTAL, 1, height, 1);

        //Average Vertical
        RenderTexture totalAverage = CreateRenderTexture(1, 1);
        computeShader.SetTexture(KERNEL_AVERAGE_VERTICAL, "_Target", totalAverage);
        computeShader.SetTexture(KERNEL_AVERAGE_VERTICAL, "_EnvironmentMap", horizontalAverage);
        computeShader.Dispatch(KERNEL_AVERAGE_VERTICAL, 1, 1, 1);

        //Divide greyscale by average;
        RenderTexture environmentPDF = CreateRenderTexture(width, height);
        computeShader.SetTexture(KERNEL_DIVIDE, "_Target", environmentPDF);
        computeShader.SetTexture(KERNEL_DIVIDE, "_A", greyScale);
        computeShader.SetInt("_A_Width", greyScale.width);
        computeShader.SetInt("_A_Height", greyScale.height);
        computeShader.SetTexture(KERNEL_DIVIDE, "_B", totalAverage);
        computeShader.SetInt("_B_Width", totalAverage.width);
        computeShader.SetInt("_B_Height", totalAverage.height);
        computeShader.Dispatch(KERNEL_DIVIDE, width / 8, height / 8, 1);

        Camera.main.GetComponent<ViewRenderTexture>().renderTexture = environmentPDF;

        //Average PDF Horizontal
        RenderTexture horizontalAveragePDF = CreateRenderTexture(1, height);
        computeShader.SetTexture(KERNEL_DIVIDE, "_Target", horizontalAveragePDF);
        computeShader.SetTexture(KERNEL_DIVIDE, "_A", horizontalAverage);
        computeShader.SetInt("_A_Width", horizontalAverage.width);
        computeShader.SetInt("_A_Height", horizontalAverage.height);
        computeShader.SetTexture(KERNEL_DIVIDE, "_B", totalAverage);
        computeShader.SetInt("_B_Width", totalAverage.width);
        computeShader.SetInt("_B_Height", totalAverage.height);
        computeShader.Dispatch(KERNEL_DIVIDE, 1, height / 8, 1);

        //Conditional PDF
        RenderTexture conditionalPDF = CreateRenderTexture(width, height, FilterMode.Point);
        computeShader.SetTexture(KERNEL_DIVIDE, "_Target", conditionalPDF);
        computeShader.SetTexture(KERNEL_DIVIDE, "_A", greyScale);
        computeShader.SetInt("_A_Width", greyScale.width);
        computeShader.SetInt("_A_Height", greyScale.height);
        computeShader.SetTexture(KERNEL_DIVIDE, "_B", horizontalAverage);
        computeShader.SetInt("_B_Width", horizontalAverage.width);
        computeShader.SetInt("_B_Height", horizontalAverage.height);
        computeShader.Dispatch(KERNEL_DIVIDE, width / 8, height / 8, 1);

        //Find CDF Marginal Inverse
        RenderTexture marginalInverse = CreateRenderTexture(1, height, FilterMode.Point);
        computeShader.SetTexture(KERNEL_CDF_MARGINAL_INVERSE, "_Target", marginalInverse);
        computeShader.SetTexture(KERNEL_CDF_MARGINAL_INVERSE, "_EnvironmentMap", horizontalAveragePDF);
        computeShader.Dispatch(KERNEL_CDF_MARGINAL_INVERSE, 1, height, 1);

        //Find CDF Conditional Inverse
        RenderTexture conditionalInverse = CreateRenderTexture(width, height);
        computeShader.SetTexture(KERNEL_CDF_CONDITIONAL_INVERSE, "_Target", conditionalInverse);
        computeShader.SetTexture(KERNEL_CDF_CONDITIONAL_INVERSE, "_EnvironmentMap", conditionalPDF);
        computeShader.Dispatch(KERNEL_CDF_CONDITIONAL_INVERSE, width / 8, height / 8, 1);

        //Prefilter diffuse map
        RenderTexture prefilteredDiffuse = CreateRenderTexture(width, height);
        computeShader.SetTexture(KERNEL_PREFILTER_DIFFUSE, "_Target", prefilteredDiffuse);
        computeShader.SetTexture(KERNEL_PREFILTER_DIFFUSE, "_EnvironmentMap", environmentMap);
        computeShader.SetTexture(KERNEL_PREFILTER_DIFFUSE, "_CDFInvX", conditionalInverse);
        computeShader.SetTexture(KERNEL_PREFILTER_DIFFUSE, "_CDFInvY", marginalInverse);
        computeShader.SetTexture(KERNEL_PREFILTER_DIFFUSE, "_JointPDF", environmentPDF);

        computeShader.Dispatch(KERNEL_PREFILTER_DIFFUSE, width / 8, height / 8, 1);

        Texture2D result = ConvertRenderTextureToTexture2D(prefilteredDiffuse);

        // Clean up
        greyScale.Release();
        Destroy(greyScale);

        horizontalAverage.Release();
        Destroy(horizontalAverage);

        totalAverage.Release();
        Destroy(totalAverage);

        environmentPDF.Release();
        Destroy(environmentPDF);

        horizontalAveragePDF.Release();
        Destroy(horizontalAverage);

        conditionalPDF.Release();
        Destroy(conditionalPDF);

        marginalInverse.Release();
        Destroy(marginalInverse);

        conditionalInverse.Release();
        Destroy(conditionalInverse);

        prefilteredDiffuse.Release();
        Destroy(prefilteredDiffuse);

        return result;
    }

    Texture2DArray GeneratePrefilteredSpecular()
    {

        int width = environmentMap.width / 2;
        int height = environmentMap.height / 2;
        RenderTexture filteredSpecularMap = new RenderTexture(
            width,
            height,
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
        computeShader.SetFloat("_Width", width);
        computeShader.SetFloat("_Height", height);
        computeShader.SetInt("_Samples", numSamples);
        computeShader.SetFloat("_MipMapLevel", mipMapLevel);


        for (int i = 0; i < numSpecularFilterLevels; i++)
        {
            float roughness = (float)i * (1.0f / (float)(numSpecularFilterLevels - 1.0));
            computeShader.SetFloat("_Roughness", roughness);
            Debug.Log(roughness);
            computeShader.SetInt("_CurrentFilterLevel", i);
            computeShader.Dispatch(KERNEL_PREFILTER_SPECULAR, width / 8, height / 8, 1);
        }

        // computeShader.Dispatch(KERNEL_PREFILTER_SPECULAR, width / 8, height / 8, numSpecularFilterLevels);

        Texture2DArray result = ConvertRenderTextureToTexture2DArray(filteredSpecularMap);

        filteredSpecularMap.Release();
        Destroy(filteredSpecularMap);

        return result;
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

    RenderTexture CreateRenderTexture(int width, int height, FilterMode filterMode = FilterMode.Bilinear)
    {
        RenderTexture rt = new RenderTexture(
                    width,
                    height,
                    0,
                    RenderTextureFormat.ARGBFloat,
                    RenderTextureReadWrite.Linear
                );
        rt.filterMode = filterMode;
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.enableRandomWrite = true;
        rt.useMipMap = false;
        rt.autoGenerateMips = false;
        rt.anisoLevel = 16;
        rt.Create();
        return rt;
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
