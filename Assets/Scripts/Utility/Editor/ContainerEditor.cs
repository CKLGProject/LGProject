using UnityEditor;
using UnityEngine.UIElements;
using Utility;

[CustomEditor(typeof(Container))]
public class ContainerEditor : Editor
{
    private VisualElement _root;

    private Container _container;

    public override VisualElement CreateInspectorGUI()
    {
        _container = target as Container;
        _root = new VisualElement();

        Button button = new Button();
        button.text = "위치 리셋";
        button.clicked += () =>
        {
            Undo.RecordObject(_container.transform, $"Reset Position - {_container.name}");
            _container.PositionReset();
            EditorUtility.SetDirty(_container);
        };

        _root.Add(button);

        return _root;
    }
}