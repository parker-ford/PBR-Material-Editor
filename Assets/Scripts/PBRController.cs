using UnityEngine;

public class PBRController : MonoBehaviour
{

    public enum NormalDistributionFunction
    {
        BlinnPhong = 0,
        Beckman = 1,
        GGX = 2,
    }

    public enum GeometryAttenuationFunction
    {
        Beckman = 0,
        GGX = 1,
        SchlickGGX = 2
    }

    public enum Diffuse
    {
        Lambert = 0,
        Hammon = 1,
        Disney = 2
    }

    public enum DebugView
    {
        None = 0,
        NormalDistributionFunction = 1,
        GeometryAttenuation = 2,
        Fresnel = 3,
        Diffuse = 4,
        Specular = 5,
    }

    public PBRView view;
    public Shader pbrShader;
    public Mesh defaultMesh;

    private GameObject model;
    private Material material;
    private Mesh mesh;
    private float reflectance = 0.0f;
    private float roughness = 0.0f;
    private float subsurface = 0.0f;
    private Color diffuseColor = new Color(1, 1, 1);
    private NormalDistributionFunction ndf;
    private GeometryAttenuationFunction geometry;
    private Diffuse diffuse;
    private DebugView debug;


    void Start()
    {
        mesh = defaultMesh;
        material = new Material(pbrShader);
        InstantiateModel();
        UpdateMaterialParameters();
    }

    void InstantiateModel()
    {
        model = new GameObject("Model");
        model.AddComponent<MeshFilter>().mesh = mesh;
        model.AddComponent<MeshRenderer>().material = material;
    }

    void UpdateMaterialParameters()
    {
        if (!material)
        {
            Debug.LogError("PBR Material is null");
            return;
        }

        material.SetFloat("_Reflectance", reflectance);
        material.SetFloat("_Roughness", roughness);
        material.SetFloat("_Subsurface", subsurface);
        material.SetColor("_DiffuseColor", diffuseColor);

        material.SetInt("_NDF", (int)ndf);
        material.SetInt("_GEO", (int)geometry);
        material.SetInt("_Diffuse", (int)diffuse);
        material.SetInt("_DebugView", (int)debug);

    }

    void Update()
    {

    }

    void OnDisable()
    {
        Destroy(material);
        Destroy(model);
    }
}
