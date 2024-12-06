using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentObject", menuName = "Scriptable Objects/EnvironmentObject")]
public class EnvironmentObject : ScriptableObject
{
    public string id;
    public Texture2D displayImage;
    public Material skyboxMaterial;
    public Texture2D filteredDiffuseMap;
    public Texture2DArray filteredSpecularMap;
}
