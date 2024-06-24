using UnityEngine;
using UnityEngine.UI;

namespace UIExtension
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("UI/Button Interactable Sprite")]
    public class ButtonInteractableSprite : MonoBehaviour
    {
        [Tooltip("활성화 되었을 때 스프라이트")]
        public Sprite ActiveSprite;
        
        [Tooltip("비활성화 되었을 때 스프라이트")]
        public Sprite InactiveSprite;

        private Button _button;
        private Image _image;

        private bool _cachedInteractable;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        private void Update()
        {
            if (_button)
            {
                if (_button.interactable != _cachedInteractable)
                {
                    _cachedInteractable = _button.interactable;
                    UpdateButtonSprite(_cachedInteractable);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        internal void UpdateButtonSprite(bool isActive)
        {
#if UNITY_EDITOR
            if (!_image)
                _image = GetComponent<Image>();
#endif
            if (isActive)
            {
                if (ActiveSprite) 
                    _image.sprite = ActiveSprite;
            }
            else
            {
                if (InactiveSprite) 
                    _image.sprite = InactiveSprite;
            }
        }
    }
}