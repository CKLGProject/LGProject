using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(NetworkBehaviour), true)]
public class NetworkBehaviourUIElement : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new();

        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        return container;
    }
}

[CustomEditor(typeof(MonoBehaviour), true)]
public class MonoBehaviourUIElement : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new();

        InspectorElement.FillDefaultInspector(container, serializedObject, this);
        return container;
    }
}
