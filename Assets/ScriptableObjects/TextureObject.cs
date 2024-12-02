using UnityEngine;

[CreateAssetMenu(fileName = "TextureObject", menuName = "Scriptable Objects/TextureObject")]
public class TextureObject : ScriptableObject
{
    public Texture2D displayImage;
    public Texture2D diffuseMap;
    public Texture2D normalMap;
    public Texture2D displacementMap;
    public Texture2D roughnessMap;
}
