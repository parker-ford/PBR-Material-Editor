using UnityEngine;

[CreateAssetMenu(fileName = "ModelObject", menuName = "Scriptable Objects/ModelObject")]
public class ModelObject : ScriptableObject
{
    public string id;
    public Mesh mesh;
    public Vector3 position = Vector3.zero;
    public Vector3 scale = Vector3.one;
    public Quaternion rotation = Quaternion.identity;
    public Texture2D displayImage;

    public void SetToTransform(Transform transform)
    {
        transform.position = position;
        transform.localScale = scale;
        transform.rotation = rotation;
    }

    public Mesh GetMesh()
    {
        return mesh;
    }
    public void SetMesh(Mesh _mesh)
    {
        mesh = _mesh;
    }
    public Vector3 GetPosition()
    {
        return position;
    }
    public void SetPosition(Vector3 _position)
    {
        position = new Vector3(_position.x, _position.y, _position.z);
    }
    public Vector3 GetScale()
    {
        return scale;
    }
    public void SetScale(Vector3 _scale)
    {
        scale = _scale;
    }
    public Quaternion GetRotation()
    {
        return rotation;
    }
    public void SetRotation(Quaternion _rotation)
    {
        rotation = _rotation;
    }

}

