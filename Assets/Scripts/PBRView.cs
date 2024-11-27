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

        var environmentButton = buttonsHolder.CreateChild<Button>("environment-butotn", "pbr-button");
        environmentButton.text = "Environment: Default";

        // var modelOverlay = container.CreateChild<PBROverlaySelector>();
    }
}
