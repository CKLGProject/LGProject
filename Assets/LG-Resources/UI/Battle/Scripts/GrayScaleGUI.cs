using UnityEditor;
using UnityEngine;

namespace NKStudio
{
    public class GrayScaleGUI : ShaderGUI
    {
        private static class Style
        {
            public static readonly GUIContent GrayScaleIntensityContent = new("GrayScale Intensity", "그레이 스케일의 세기를 조절합니다.");
            public static readonly GUIContent MixDarkIntensityContent = new("Mix Dark Intensity", "어두운 색을 섞을 비율을 조절합니다.");
        }
        
        private MaterialProperty _grayScaleIntensityProperty;
        private MaterialProperty _mixDarkIntensity;
        
        private void FindProperty(MaterialProperty[] properties)
        {
            _grayScaleIntensityProperty = FindProperty("_Intensity", properties);
            _mixDarkIntensity = FindProperty("_MixDarkIntensity", properties);
        }
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            FindProperty(properties);
            
            materialEditor.ShaderProperty(_grayScaleIntensityProperty, Style.GrayScaleIntensityContent);
            materialEditor.ShaderProperty(_mixDarkIntensity, Style.MixDarkIntensityContent);
        }
    }
}
