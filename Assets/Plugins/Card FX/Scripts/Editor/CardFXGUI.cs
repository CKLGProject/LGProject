using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public class CardFXGUI : ShaderGUI
    {
        private MaterialProperty _widthProperty;
        private MaterialProperty _heightProperty;
        private MaterialProperty _radiusProperty;
        private MaterialEditor _materialEditor;

        private void FindProperties(MaterialProperty[] properties)
        {
            _widthProperty = FindProperty("_Width", properties);
            _heightProperty = FindProperty("_Height", properties);
            _radiusProperty = FindProperty("_Radius", properties);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _materialEditor = materialEditor;
            FindProperties(properties);

            ShaderPropertiesGUI();
        }

        private void ShaderPropertiesGUI()
        {
            DrawHeader("Card FX");

            EditorGUI.BeginChangeCheck();
            {
                EditorGUIUtility.fieldWidth = 64f;

                EditorGUILayout.Separator();

                InspectorBox(10, () =>
                {
                    EditorGUILayout.LabelField(new GUIContent("Option"), EditorStyles.boldLabel);
                    GUILayout.Space(5);

                    _materialEditor.ShaderProperty(_widthProperty, "Width");
                    _materialEditor.ShaderProperty(_heightProperty, "Height");
                    _materialEditor.ShaderProperty(_radiusProperty, "Radius");
                    
                });
            }
        }

        private void DrawHeader(string name)
        {
            // Init
            GUIStyle rolloutHeaderStyle = new(GUI.skin.box);
            rolloutHeaderStyle.fontStyle = FontStyle.Bold;
            rolloutHeaderStyle.fontSize = 18;
            rolloutHeaderStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            // Draw
            GUILayout.Label(name, rolloutHeaderStyle, GUILayout.Height(24), GUILayout.ExpandWidth(true));
        }

        private static void InspectorBox(int aBorder, System.Action inside)
        {
            Rect r = EditorGUILayout.BeginHorizontal();

            GUI.Box(r, GUIContent.none);
            GUILayout.Space(aBorder);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(aBorder);
            inside();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndVertical();
            GUILayout.Space(aBorder);
            EditorGUILayout.EndHorizontal();
        }
    }
}