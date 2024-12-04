using System;
using UnityEngine;

public class ViewRenderTexture : MonoBehaviour
{
    public Texture2D tex;
    public RenderTexture renderTexture;
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (renderTexture != null)
        {
            Graphics.Blit(tex, destination);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

}
