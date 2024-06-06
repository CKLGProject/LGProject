using Data;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour
{
    public ECharacterType CharacterType;
    public Sprite DefaultProfileImage;
    public Sprite SelectedProfileImage;

    [SerializeField] private Image targetBackground;

    public SerializableReactiveProperty<bool> IsSelected { get; private set; } = new SerializableReactiveProperty<bool>(false);

    private void Start()
    {
        IsSelected
            .Subscribe(active => targetBackground.sprite = active ? SelectedProfileImage : DefaultProfileImage)
            .AddTo(this);
    }
    
    /// <summary>
    /// 선택 상태를 설정합니다.
    /// </summary>
    /// <param name="isSelected">상태</param>
    public void SetSelected(bool isSelected)
    {
        IsSelected.Value = isSelected;
    }
}