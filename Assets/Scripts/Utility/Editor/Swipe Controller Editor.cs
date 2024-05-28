using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(SwipeController))]
public class SwipeControllerEditor : Editor
{
    private SerializedProperty _horizontalFilterType;
    private SerializedProperty _verticalFilterType;
    private SerializedProperty _swipeHorizontalSensitivity;
    private SerializedProperty _swipeVerticalSensitivity;

    private PropertyField _horizontalFilterTypeField;
    private PropertyField _verticalFilterTypeField;
    private PropertyField _swipeHorizontalSensitivityField;
    private PropertyField _swipeVerticalSensitivityField;

    private VisualElement _root;

    private void FindProperties()
    {
        _horizontalFilterType = serializedObject.FindProperty("HorizontalFilterType");
        _verticalFilterType = serializedObject.FindProperty("VerticalFilterType");
        _swipeHorizontalSensitivity = serializedObject.FindProperty("SwipeHorizontalSensitivity");
        _swipeVerticalSensitivity = serializedObject.FindProperty("SwipeVerticalSensitivity");
    }

    private void Init()
    {
        FindProperties();

        _root = new VisualElement();
        _horizontalFilterTypeField = new PropertyField(_horizontalFilterType);
        _verticalFilterTypeField = new PropertyField(_verticalFilterType);
        _swipeHorizontalSensitivityField = new PropertyField(_swipeHorizontalSensitivity);
        _swipeVerticalSensitivityField = new PropertyField(_swipeVerticalSensitivity);

        _root.Add(_horizontalFilterTypeField);
        _root.Add(_verticalFilterTypeField);
        _root.Add(_swipeHorizontalSensitivityField);
        _root.Add(_swipeVerticalSensitivityField);

        SwipeController.EHorizontalFilterType horizontalFilterType =
            (SwipeController.EHorizontalFilterType)_horizontalFilterType.intValue;

        if (horizontalFilterType == SwipeController.EHorizontalFilterType.None)
            _swipeHorizontalSensitivityField.style.display = DisplayStyle.None;

        SwipeController.EVerticalFilterType verticalFilterType =
            (SwipeController.EVerticalFilterType)_verticalFilterType.intValue;

        if (verticalFilterType == SwipeController.EVerticalFilterType.None)
            _swipeVerticalSensitivityField.style.display = DisplayStyle.None;
    }

    public override VisualElement CreateInspectorGUI()
    {
        Init();

        _horizontalFilterTypeField.RegisterValueChangeCallback(OnChangeHorizontalType);
        _verticalFilterTypeField.RegisterValueChangeCallback(OnChangeVerticalType);
        
        return _root;
    }
    
    private void OnChangeHorizontalType(SerializedPropertyChangeEvent evt)
    {
        SwipeController.EHorizontalFilterType horizontalFilterType =
            (SwipeController.EHorizontalFilterType)evt.changedProperty.intValue;

        if (horizontalFilterType == SwipeController.EHorizontalFilterType.None)
            _swipeHorizontalSensitivityField.style.display = DisplayStyle.None;
        else
            _swipeHorizontalSensitivityField.style.display = DisplayStyle.Flex;
    }

    private void OnChangeVerticalType(SerializedPropertyChangeEvent evt)
    {
        SwipeController.EVerticalFilterType verticalFilterType =
            (SwipeController.EVerticalFilterType)evt.changedProperty.intValue;

        if (verticalFilterType == SwipeController.EVerticalFilterType.None)
            _swipeVerticalSensitivityField.style.display = DisplayStyle.None;
        else
            _swipeVerticalSensitivityField.style.display = DisplayStyle.Flex;
    }
}