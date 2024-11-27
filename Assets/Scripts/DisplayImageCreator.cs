using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DisplayImageCreator : MonoBehaviour
{
    private Material material;
    private List<ModelObject> models;
    private GameObject displayModel;
    private ModelObject currentModel;
    private int modelIndex;
    private const string path = "Assets/DisplayImages/";
    void Start()
    {
        models = Loader.LoadScriptableObjects<ModelObject>();
        modelIndex = 0;
        displayModel = new GameObject("Display Model");
        displayModel.AddComponent<MeshFilter>();
        material = new Material(Shader.Find("Standard"));
        displayModel.AddComponent<MeshRenderer>().material = material;
        NextModel();
    }

    void OnDisable()
    {
        Destroy(displayModel);
    }

    void NextModel()
    {
        currentModel = models[modelIndex];
        currentModel.SetToTransform(displayModel.transform);
        displayModel.GetComponent<MeshFilter>().mesh = currentModel.mesh;
        modelIndex++;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentModel.position = displayModel.transform.position;
            currentModel.scale = displayModel.transform.localScale;
            currentModel.rotation = displayModel.transform.rotation;
            SaveDisplayImage();

            if (modelIndex < models.Count)
            {
                NextModel();
            }
            else
            {
                EditorApplication.isPlaying = false;
            }

        }
    }

    void SaveDisplayImage()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        Camera.main.targetTexture = null;
        Destroy(renderTexture);

        byte[] bytes = texture.EncodeToPNG();
        string fullPath = path + currentModel.id + ".png";
        File.WriteAllBytes(fullPath, bytes);
        Debug.Log(fullPath + " saved");
        currentModel.displayImagePath = fullPath;
    }
}
