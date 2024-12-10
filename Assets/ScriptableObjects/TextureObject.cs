using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureObject", menuName = "Scriptable Objects/TextureObject")]
public class TextureObject : ScriptableObject
{
    public String id;
    public Texture2D displayImage;
    public Texture2D diffuseMap;
    public Texture2D normalMap;
    public Texture2D displacementMap;
    public Texture2D roughnessMap;
    public Texture2D metallicMap;
}
