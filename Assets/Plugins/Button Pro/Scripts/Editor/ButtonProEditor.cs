using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(ButtonPro))]
public class ButtonProEditor : ButtonEditor
{
    private SerializedProperty _syncTarget;

    private static GUIContent _otherTargetContent;

    private ButtonPro _buttonPro;

    private const string IconName = "Button Icon";

    protected override void OnEnable()
    {
        base.OnEnable();
        _buttonPro = target as ButtonPro;
        
        ApplyIcon(IconName, _buttonPro);
        FindProperty();
    }

    private void FindProperty()
    {
        _syncTarget = serializedObject.FindProperty("SyncTarget");

        string message = Application.systemLanguage == SystemLanguage.Korean
            ? "버튼 컬러의 영향을 받을 다른 객체를 추가합니다."
            : "Add other objects that will be affected by the button color.";

        _otherTargetContent = new GUIContent("Sync Other Target", message);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Ensure the serialized object is up-to-date
        base.OnInspectorGUI();

        EditorGUILayout.Space();


        if (_buttonPro.transition == Selectable.Transition.ColorTint)
        {
            EditorGUILayout.LabelField("Advance");
            ++EditorGUI.indentLevel;
            EditorGUILayout.PropertyField(_syncTarget, _otherTargetContent);
            --EditorGUI.indentLevel;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private static void ApplyIcon(string iconPath, Object targetObject)
    {
        string prefix = EditorGUIUtility.isProSkin ? "d_" : "";
        
        GUIContent icon = EditorGUIUtility.IconContent(prefix+iconPath);
        EditorGUIUtility.SetIconForObject(targetObject, (Texture2D)icon.image);
    }
}