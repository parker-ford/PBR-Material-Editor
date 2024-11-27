using UnityEngine;

[CreateAssetMenu(fileName = "ModelObject", menuName = "Scriptable Objects/ModelObject")]
public class ModelObject : ScriptableObject
{
    public string id;
    public Mesh mesh;
    public Vector3 position = new Vector3(0, 0, 0);
    public Vector3 scale = new Vector3(1, 1, 1);
    public Quaternion rotation;
    public string displayImagePath;

    public void SetToTransform(Transform transform)
    {
        transform.position = position;
        transform.localScale = scale;
        transform.rotation = rotation;
    }
}

