using UnityEngine;
using UnityEngine.UIElements;
using MaterialUI;

public class PBRView : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private StyleSheet style;

    private VisualElement root;
    private VisualElement container;
    public MaterialMenu materialMenu;

    public void GenerateUI()
    {
        root = document.rootVisualElement;
        root.Clear();
        root.styleSheets.Add(style);

        container = root.CreateChild("container");

        var buttonsHolder = container.CreateChild("buttons-holder");

        materialMenu = buttonsHolder.CreateChild<MaterialMenu>("material-menu");

        var modelButton = buttonsHolder.CreateChild<Button>("models-button", "pbr-button");
        modelButton.text = "Model: Sphere";
        var modelOverlay = container.CreateChild<PBROverlaySelector>();
        modelOverlay.label.text = "Models";
        modelOverlay.style.display = DisplayStyle.None;
        modelButton.clicked += () => modelOverlay.style.display = DisplayStyle.Flex;

        var textureButton = buttonsHolder.CreateChild<Button>("texture-button", "pbr-button");
        textureButton.text = "Texture: None";
        var textureOverlay = container.CreateChild<PBROverlaySelector>();
        textureOverlay.label.text = "Textures";
        textureOverlay.style.display = DisplayStyle.None;
        textureButton.clicked += () => textureOverlay.style.display = DisplayStyle.Flex;

        var environmentButton = buttonsHolder.CreateChild<Button>("environment-butotn", "pbr-button");
        environmentButton.text = "Environment: Default";
        var environmentOverlay = container.CreateChild<PBROverlaySelector>();
        environmentOverlay.label.text = "Environments";
        environmentOverlay.style.display = DisplayStyle.None;
        environmentButton.clicked += () => environmentOverlay.style.display = DisplayStyle.Flex;

        var presetsButton = buttonsHolder.CreateChild<Button>("presets-button", "pbr-button");
        presetsButton.text = "Presets";
        var presetsOverlay = container.CreateChild<PBROverlaySelector>();
        presetsOverlay.label.text = "Presets";
        presetsOverlay.style.display = DisplayStyle.None;
        presetsButton.clicked += () => presetsOverlay.style.display = DisplayStyle.Flex;
    }
}
