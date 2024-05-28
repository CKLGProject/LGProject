using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PatCaptureDebugWindow : EditorWindow
{
    [SerializeField] private VisualTreeAsset m_VisualTreeAsset;

    private Slider _intensitySlider;
    private Slider _alphaSlider;

    private static readonly int Intensity = Shader.PropertyToID("_LightIntensity");
    private static readonly int Alpha = Shader.PropertyToID("_LightAlpha");

    [MenuItem("Tools/Pat Capture Debug Window")]
    public static void ShowWindow()
    {
        PatCaptureDebugWindow wnd = GetWindow<PatCaptureDebugWindow>();
        wnd.titleContent = new GUIContent("Pat Capture Debug Window");
        wnd.minSize = new Vector2(300, 100);
        wnd.maxSize = new Vector2(300, 100);
    }

    private void BindProperty(VisualElement root)
    {
        float intensity = Shader.GetGlobalFloat(Intensity);
        float alpha = Shader.GetGlobalFloat(Alpha);
        
        _intensitySlider = root.Q<Slider>("intensity-slider");
        _intensitySlider.SetValueWithoutNotify(intensity);
        _intensitySlider.highValue = 1;
        _intensitySlider.lowValue = 0;
        
        _alphaSlider = root.Q<Slider>("alpha-slider");
        _alphaSlider.SetValueWithoutNotify(alpha);
        _alphaSlider.highValue = 1;
        _alphaSlider.lowValue = 0;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement uxml = m_VisualTreeAsset.Instantiate();
        root.Add(uxml);

        BindProperty(root);

        _intensitySlider.RegisterValueChangedCallback(evt =>
        {
            float intensity = evt.newValue;
            Shader.SetGlobalFloat(Intensity, intensity);
        });
        
        _alphaSlider.RegisterValueChangedCallback(evt =>
        {
            float alpha = evt.newValue;
            Shader.SetGlobalFloat(Alpha, alpha);
        });
    }
}