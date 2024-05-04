using UnityEditor;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(UnityEngine.MonoBehaviour), true)]
public class MonoBehaviourUIElement : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new();

        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        return container;
    }
}
