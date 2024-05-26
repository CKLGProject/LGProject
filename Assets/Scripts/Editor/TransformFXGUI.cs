using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace FXEditor
{
    public class TransformFXGUI : ShaderGUI
    {
        private MaterialProperty _useLocalProperty;
        private MaterialProperty _localLightAlphaProperty;
        private MaterialProperty _localLightIntensity;

        private GUIContent _localLightIntensityGUIContent = new GUIContent("Local Light Intensity");

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _useLocalProperty = FindProperty("_USELOCAL", properties);
            _localLightAlphaProperty = FindProperty("_LocalLightAlpha", properties);
            _localLightIntensity = FindProperty("_LocalLightIntensity", properties);

            materialEditor.ShaderProperty(_useLocalProperty, _useLocalProperty.displayName);

            if ((int)_useLocalProperty.floatValue == 1)
            {
                materialEditor.MinFloatShaderProperty(_localLightIntensity, _localLightIntensityGUIContent, 0);
                materialEditor.ShaderProperty(_localLightAlphaProperty, _localLightAlphaProperty.displayName);
            }
        }
    }
}