using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIExtension
{
    [CustomEditor(typeof(ButtonInteractableSprite))]
    public class ButtonInteractableSpriteEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            ButtonInteractableSprite buttonInteractableSprite = target as ButtonInteractableSprite;

            VisualElement container = new VisualElement();
            VisualElement defaultInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultInspector, serializedObject, this);

            container.Add(defaultInspector);

            // 플레이 모드에서는 동작하지 않도록 처리
            if (Application.isPlaying)
                return container;

            if (buttonInteractableSprite)
            {
                GameObject testFXGameObject = buttonInteractableSprite.gameObject;
                UnityEngine.UI.Button button = testFXGameObject.GetComponent<UnityEngine.UI.Button>();

                if (button)
                {
                    SerializedObject buttonSerializedObject = new(button);
                    SerializedProperty interactableProperty = buttonSerializedObject.FindProperty("m_Interactable");

                    container.TrackPropertyValue(interactableProperty, property =>
                    {
                        bool newInteractable = property.boolValue;
                        buttonInteractableSprite.UpdateButtonSprite(newInteractable);
                        SceneView.RepaintAll();
                    });
                }
            }

            return container;
        }
    }
}