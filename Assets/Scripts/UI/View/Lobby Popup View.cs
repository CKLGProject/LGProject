using Data;
using R3;
using R3.Triggers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.Singleton;
using UnityEngine.UI;

public class LobbyPopupView : MonoBehaviour
{
    [Header("팝업")] [SerializeField] private GameObject popupView;

    [Header("캐릭터 이미지 그룹")] [FormerlySerializedAs("HitImage"), SerializeField]
    private GameObject hitImage;

    [FormerlySerializedAs("FrostImage"), SerializeField]
    private GameObject frostImage;

    [FormerlySerializedAs("CaneImage"), SerializeField]
    private GameObject kaneImage;

    [Header("캐릭터 이름 그룹")] [FormerlySerializedAs("HitName"), SerializeField]
    private GameObject hitName;

    [FormerlySerializedAs("FrostName"), SerializeField]
    private GameObject frostName;

    [FormerlySerializedAs("CaneName"), SerializeField]
    private GameObject kaneName;

    [Header("펫 이미지")] [SerializeField] private Image petImage;

    [Header("Text")] [FormerlySerializedAs("attackText"), SerializeField]
    private TextMeshProUGUI attackPowerText;

    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI attackSpeedText;

    [Header("캐릭터 프로필")] [SerializeField] private CharacterProfile[] characterProfiles;

    [Header("펫 프로필")] [SerializeField] private PetData[] petDataList;

    [Header("캐릭터 프로필 버튼")] [SerializeField]
    private Button hitProfileButton;

    [SerializeField] private Button frostProfileButton;
    [SerializeField] private Button kaneProfileButton;
    [SerializeField] private Button stomeProfileButton;
    [SerializeField] private Button brightProfileButton;

    [Header("버튼")] [SerializeField] private Button characterSelectionButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button petChoiceButton;
    [SerializeField] private Button itemChoiceButton;

    private Subject<bool> _popupViewActiveSubject = new Subject<bool>();

    private void Start()
    {
        // 초기에는 비활성화 상태
        popupView.SetActive(false);

        popupView.OnEnableAsObservable()
            .Subscribe(_ => _popupViewActiveSubject.OnNext(true))
            .AddTo(this);

        popupView.OnDisableAsObservable()
            .Subscribe(_ => _popupViewActiveSubject.OnNext(false))
            .AddTo(this);
    }

    /// <summary>
    /// 모든 프로필을 비활성화 상태로 만듭니다.
    /// </summary>
    public void AllNoneSelected()
    {
        foreach (CharacterProfile profile in characterProfiles)
        {
            profile.SetSelected(false);
        }
    }

    /// <summary>
    /// 현재 선택된 녀석을 활성화 합니다.
    /// </summary>
    /// <param name="characterType"></param>
    public void ActiveCurrentCharacterProfile(ECharacterType characterType)
    {
        foreach (CharacterProfile profile in characterProfiles)
        {
            if (profile.CharacterType != characterType)
                continue;

            profile.SetSelected(true);
            return;
        }
    }

    public Observable<Unit> HitProfileButtonClicked()
    {
        Assert.IsNotNull(hitProfileButton, "hitProfileButton != null");

        return hitProfileButton.OnClickAsObservable();
    }

    public Observable<Unit> FrostProfileButtonClicked()
    {
        Assert.IsNotNull(frostProfileButton, "frostProfileButton != null");

        return frostProfileButton.OnClickAsObservable();
    }

    public Observable<Unit> KaneProfileButtonClicked()
    {
        Assert.IsNotNull(kaneProfileButton, "kaneProfileButton != null");

        return kaneProfileButton.OnClickAsObservable();
    }

    public Observable<Unit> StomeProfileButtonClicked()
    {
        Assert.IsNotNull(stomeProfileButton, "stomeProfileButton != null");

        return stomeProfileButton.OnClickAsObservable();
    }

    public Observable<Unit> brightProfileButtonClicked()
    {
        Assert.IsNotNull(brightProfileButton, "brightProfileButton != null");

        return brightProfileButton.OnClickAsObservable();
    }


    /// <summary>
    /// 캐릭터 선택 버튼의 상호작용을 설정합니다.
    /// </summary>
    /// <param name="active">활성화 상태</param>
    public void SetInteractionByCharacterSelectionButton(bool active)
    {
        characterSelectionButton.interactable = active;
    }

    /// <summary>
    /// 해당 카드 이미지 활성화
    /// </summary>
    /// <param name="characterType">캐릭터 타입</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetActiveCharacterImage(ECharacterType characterType)
    {
        // 모두 비활성화
        hitImage.SetActive(false);
        frostImage.SetActive(false);
        kaneImage.SetActive(false);

        hitName.SetActive(false);
        frostName.SetActive(false);
        kaneName.SetActive(false);

        switch (characterType)
        {
            case ECharacterType.Hit:
                hitImage.SetActive(true);
                hitName.SetActive(true);
                break;
            case ECharacterType.Frost:
                frostImage.SetActive(true);
                frostName.SetActive(true);
                break;
            case ECharacterType.Kane:
                kaneImage.SetActive(true);
                kaneName.SetActive(true);
                break;
            case ECharacterType.Storm:
                break;
            case ECharacterType.E:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
        }
    }

    public Observable<Unit> OnCloseButtonClicked()
    {
        Assert.IsNotNull(exitButton, "exitButton != null");

        return exitButton.OnClickAsObservable();
    }

    public Observable<Unit> OnCharacterSelectionButtonClicked()
    {
        Assert.IsNotNull(characterSelectionButton, "characterSelectionButton != null");

        return characterSelectionButton.OnClickAsObservable();
    }

    public Observable<Unit> OnPetChoiceButtonClicked()
    {
        Assert.IsNotNull(petChoiceButton, "petChoiceButton != null");

        return petChoiceButton.OnClickAsObservable();
    }

    public Observable<Unit> OnItemChoiceButtonClicked()
    {
        Assert.IsNotNull(itemChoiceButton, "itemChoiceButton != null");

        return itemChoiceButton.OnClickAsObservable();
    }

    /// <summary>
    ///  팝업 뷰를 활성화 상태를 설정합니다.
    /// </summary>
    /// <param name="active"></param>
    public void SetActivePopupView(bool active)
    {
        popupView.SetActive(active);
    }

    /// <summary>
    /// 캐릭터 데이터를 설정합니다.
    /// </summary>
    /// <param name="data"></param>
    public void SetCharacterData(CharacterData data)
    {
        attackPowerText.text = data.AttackPower.ToString();
        defenseText.text = data.DefensePower.ToString();
        moveSpeedText.text = data.MoveSpeed.ToString();
        attackSpeedText.text = data.AttackSpeed.ToString();
    }

    /// <summary>
    /// 팝업 뷰 활성화 상태에 대한 옵저버블을 반환합니다.
    /// </summary>
    /// <returns></returns>
    public Observable<bool> OnPopupViewActiveObservable()
    {
        return _popupViewActiveSubject;
    }

    /// <summary>
    /// 캐릭터 타입에 알맞는 펫을 활성화 시키는 함수
    /// </summary>
    /// <param name="characterType">펫을 활성화해야 하는 캐릭터의 유형</param>
    public void ActivePet(ECharacterType characterType)
    {
        // 일단 끔
        petChoiceButton.gameObject.SetActive(false);
        
        switch (characterType)
        {
            case ECharacterType.Hit:
                
                if (TryPetActive(EPetType.Scorchwing))
                    return;
                
                break;
            case ECharacterType.Frost:
                
                if (TryPetActive(EPetType.Icebound))
                    return;
                
                break;
            case ECharacterType.Kane:
                
                if (TryPetActive(EPetType.Aerion))
                    return;
                break;
            case ECharacterType.Storm:
                
                if (TryPetActive(EPetType.Aerion))
                    return;
                
                break;
            case ECharacterType.None:
            case ECharacterType.E:
            default:
                throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
            
        }
        
        // 펫이 없거나 활성화 할 수 없는 경우
        petChoiceButton.gameObject.SetActive(true);
        petImage.gameObject.SetActive(false);
        petImage.sprite = null;
    }
    
    /// <summary>
    /// 해당 펫을 활성화 시킬 수 있다면 활성화를 시키고 그에 대한 결과를 반환합니다.
    /// </summary>
    /// <param name="petType">활성화 시킬 펫</param>
    /// <returns>성공 여부</returns>
    private bool TryPetActive(EPetType petType)
    {
        bool hasPet = Singleton.Instance<GameManager>().HasPet(petType);
        if (hasPet)
        {
            bool isEnablePet = Singleton.Instance<GameManager>().IsEnablePet(petType);
            if (isEnablePet)
            {
                // 개발 기간상 0단계 펫만 존재
                const int petLevel = 0;

                petImage.gameObject.SetActive(true);
                petImage.sprite = petDataList[(int)petType].PetProfileImage[petLevel];
                return true;
            }
        }

        return false;
    }
}