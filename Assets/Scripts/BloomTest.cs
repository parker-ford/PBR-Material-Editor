using UnityEngine;
using UnityEngine.Rendering;

public class BloomTest : MonoBehaviour
{
    public Texture2D inputImage;
    public ComputeShader fftShader;
    private RenderTexture renderTexture;
    private RenderTexture kernel;
    public bool showRenderTexture;
    private int KERNEL_HORIZONTAL_FFT;
    private int KERNEL_VERTICAL_FFT;
    private int KERNEL_HORIZONTAL_IFFT;
    private int KERNEL_VERTICAL_IFFT;
    private int KERNEL_POST_PROCESS;
    private int KERNEL_PRE_PROCESS;
    private int KERNEL_BLOOM;
    private int KERNEL_CONVOLVE;

    // void IFFT(RenderTexture input)
    // {
    //     fftShader.SetTexture(KERNEL_HORIZONTAL_IFFT, "_Target", input);
    //     fftShader.Dispatch(KERNEL_HORIZONTAL_IFFT, 1, size, 1);

    //     fftShader.SetTexture(KERNEL_VERTICAL_IFFT, "_Target", input);
    //     fftShader.Dispatch(KERNEL_VERTICAL_IFFT, 1, size, 1);

    //     fftShader.SetBool("_Scale", false);
    //     fftShader.SetBool("_Permute", true);
    //     fftShader.SetTexture(KERNEL_POST_PROCESS, "_Target", input);
    //     fftShader.Dispatch(KERNEL_POST_PROCESS, size, size, 1);
    // }

    public static RenderTexture CreateRenderTexture(
            RenderTextureDescriptor descriptor,
            TextureWrapMode wrapMode, FilterMode filterMode,
            int anisoLevel)
    {
        RenderTexture rt = new RenderTexture(descriptor)
        {
            anisoLevel = filterMode == FilterMode.Trilinear ? anisoLevel : 0,
            wrapMode = wrapMode,
            filterMode = filterMode
        };
        rt.Create();
        return rt;
    }
    void Start()
    {
        KERNEL_HORIZONTAL_FFT = fftShader.FindKernel("CS_HorzontalStepFFT");
        KERNEL_VERTICAL_FFT = fftShader.FindKernel("CS_VerticalStepFFT");
        KERNEL_HORIZONTAL_IFFT = fftShader.FindKernel("CS_HorzontalStepIFFT");
        KERNEL_VERTICAL_IFFT = fftShader.FindKernel("CS_VerticalStepIFFT");
        KERNEL_POST_PROCESS = fftShader.FindKernel("CS_PostProcess");
        KERNEL_PRE_PROCESS = fftShader.FindKernel("CS_PreProcess");
        KERNEL_BLOOM = fftShader.FindKernel("CS_BloomKernel");
        KERNEL_CONVOLVE = fftShader.FindKernel("CS_Convolve");

        int size = 256;
        int cascadesNumber = 1;

        RenderTextureDescriptor initialsDescriptor = new RenderTextureDescriptor()
        {
            height = size,
            width = size,
            volumeDepth = cascadesNumber,
            enableRandomWrite = true,
            colorFormat = RenderTextureFormat.ARGBHalf,
            sRGB = false,
            msaaSamples = 1,
            depthBufferBits = 0,
            useMipMap = false,
            dimension = TextureDimension.Tex2D
            // dimension = TextureDimension.Tex2DArray
        };

        kernel = CreateRenderTexture(initialsDescriptor, TextureWrapMode.Repeat, FilterMode.Point, 0);

        fftShader.SetTexture(KERNEL_BLOOM, "_Target", kernel);
        fftShader.Dispatch(KERNEL_BLOOM, size / 8, size / 8, 1);


        // renderTexture = new RenderTexture(inputImage.width, inputImage.height, 24, RenderTextureFormat.DefaultHDR);
        // renderTexture = new RenderTexture(size, size, 24, RenderTextureFormat.ARGBFloat);
        // renderTexture.enableRandomWrite = true;
        renderTexture = CreateRenderTexture(initialsDescriptor, TextureWrapMode.Repeat, FilterMode.Point, 0);
        Graphics.Blit(inputImage, renderTexture);

        fftShader.SetBool("_Permute", true);

        fftShader.SetTexture(KERNEL_PRE_PROCESS, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_PRE_PROCESS, size / 8, size / 8, 1);

        fftShader.SetBool("_Inverse", false);

        fftShader.SetTexture(KERNEL_HORIZONTAL_FFT, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_HORIZONTAL_FFT, 1, renderTexture.height, 1);

        fftShader.SetTexture(KERNEL_VERTICAL_FFT, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_VERTICAL_FFT, 1, renderTexture.width, 1);

        fftShader.SetTexture(KERNEL_CONVOLVE, "_Target", renderTexture);
        fftShader.SetTexture(KERNEL_CONVOLVE, "_Kernel", kernel);
        fftShader.Dispatch(KERNEL_CONVOLVE, size / 8, size / 8, 1);

        fftShader.SetBool("_Inverse", true);

        fftShader.SetTexture(KERNEL_VERTICAL_IFFT, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_VERTICAL_IFFT, 1, renderTexture.width, 1);

        fftShader.SetTexture(KERNEL_HORIZONTAL_IFFT, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_HORIZONTAL_IFFT, 1, renderTexture.height, 1);

        fftShader.SetBool("_Scale", true);
        // fftShader.SetBool("_Permute", true);
        fftShader.SetTexture(KERNEL_POST_PROCESS, "_Target", renderTexture);
        fftShader.Dispatch(KERNEL_POST_PROCESS, size / 8, size / 8, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (showRenderTexture)
        {
            Graphics.Blit(renderTexture, destination);
        }
        else
        {
            Graphics.Blit(inputImage, destination);
        }
    }
}
