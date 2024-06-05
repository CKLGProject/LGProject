using R3;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LobbyPopupView : MonoBehaviour
{
    [Header("캐릭터 이미지 그룹")]
    [SerializeField] private GameObject HitImage;
    [SerializeField] private GameObject FrostImage;
    [SerializeField] private GameObject CaneImage;
    
    [Header("캐릭터 프로필 버튼")]
    [SerializeField] private Button HitProfileButton;
    [SerializeField] private Button FrostProfileButton;
    [SerializeField] private Button CaneProfileButton;
    
    [Header("캐릭터 선택 버튼")]
    [SerializeField] private Button characterSelectionButton;
    
    

    public Observable<Unit> OnCharacterSelectionButtonClicked()
    {
        Assert.IsNotNull(characterSelectionButton, "characterSelectionButton != null");
        
        return characterSelectionButton.OnClickAsObservable();
    }
}