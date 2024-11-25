using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MaterialUI
{
    public class MaterialEditorView : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private StyleSheet style;

        private VisualElement root;
        private VisualElement container;

        void Start()
        {
            GenerateUI();
        }

        private void GenerateUI()
        {
            root = document.rootVisualElement;
            root.Clear();
            root.styleSheets.Add(style);

            container = root.CreateChild("container");

            var materialEditorMenu = container.CreateChild<Foldout>("material-menu");
            materialEditorMenu.text = "Material Properties";

            var diffuseProperties = materialEditorMenu.CreateChild<MaterialPropertyGroup>();
            diffuseProperties.label.text = "Diffuse Properties";

            var testSlider = diffuseProperties.CreateChild<MaterialSlider>();

            // var diffuseProperties = materialEditorMenu.CreateChild("property-group");
            // var diffuseLabel = diffuseProperties.CreateChild<Label>();
            // diffuseLabel.text = "Diffuse Properties";
            // var diffuseBaseColor = diffuseProperties.CreateChild<ColorField>("color-picker");
            // diffuseBaseColor.value = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            // diffuseBaseColor.showAlpha = true;
            // diffuseBaseColor.showEyeDropper = true;




        }


        void Update()
        {

        }
    }

}
