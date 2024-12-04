using UnityEngine;

[CreateAssetMenu(fileName = "EnvironmentObject", menuName = "Scriptable Objects/EnvironmentObject")]
public class EnvironmentObject : ScriptableObject
{
    public string id;
    public Texture2D displayImage;
    public Texture2D environmentMap;
    public Material skyboxMaterial;
    public Texture2DArray filteredSpecularMaap;
}
