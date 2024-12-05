using System;
using UnityEngine;

public class ViewRenderTexture : MonoBehaviour
{
    public Shader textureView;
    public Texture2D tex;
    public RenderTexture renderTexture;

    private Material material;

    void Start()
    {
        material = new Material(textureView);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture != null)
        {
            Graphics.Blit(renderTexture, destination, material);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

}
