using UnityEngine;
using UnityEngine.UIElements;
using MaterialUI;

public class PBRView : MonoBehaviour
{
    [SerializeField] private UIDocument document;
    [SerializeField] private StyleSheet style;

    private VisualElement root;
    public VisualElement container;
    public MaterialMenu materialMenu;

    public PBROverlaySelector modelOverlay;
    public PBROverlaySelector textureOverlay;
    public PBROverlaySelector environmentOverlay;
    public PBROverlaySelector presetsOverlay;
    public Button modelButton;
    public Button textureButton;
    public Button environmentButton;
    public Button presetsButton;

    public void GenerateUI()
    {
        root = document.rootVisualElement;
        root.Clear();
        root.styleSheets.Add(style);

        container = root.CreateChild("container");

        var buttonsHolder = container.CreateChild("buttons-holder");

        materialMenu = buttonsHolder.CreateChild<MaterialMenu>("material-menu");

        modelButton = buttonsHolder.CreateChild<Button>("models-button", "pbr-button");
        modelButton.text = "Model: Sphere";
        modelOverlay = container.CreateChild<PBROverlaySelector>();
        modelOverlay.label.text = "Models";
        modelOverlay.style.display = DisplayStyle.None;
        modelButton.clicked += () => modelOverlay.style.display = DisplayStyle.Flex;

        textureButton = buttonsHolder.CreateChild<Button>("texture-button", "pbr-button");
        textureButton.text = "Texture: None";
        textureOverlay = container.CreateChild<PBROverlaySelector>();
        textureOverlay.label.text = "Textures";
        textureOverlay.style.display = DisplayStyle.None;
        textureButton.clicked += () => textureOverlay.style.display = DisplayStyle.Flex;

        environmentButton = buttonsHolder.CreateChild<Button>("environment-butotn", "pbr-button");
        environmentButton.text = "Environment: Default";
        environmentOverlay = container.CreateChild<PBROverlaySelector>();
        environmentOverlay.label.text = "Environments";
        environmentOverlay.style.display = DisplayStyle.None;
        environmentButton.clicked += () => environmentOverlay.style.display = DisplayStyle.Flex;

        presetsButton = buttonsHolder.CreateChild<Button>("presets-button", "pbr-button");
        presetsButton.text = "Presets";
        presetsOverlay = container.CreateChild<PBROverlaySelector>();
        presetsOverlay.label.text = "Presets";
        presetsOverlay.style.display = DisplayStyle.None;
        presetsButton.clicked += () => presetsOverlay.style.display = DisplayStyle.Flex;
    }
}
