using Data;
using R3;
using UnityEngine;
using UnityEngine.Singleton;

public class LobbyPopupModel : MonoBehaviour
{
    private ReactiveProperty<ECharacterType> _selectedCharacterType;
    public ECharacterType SelectedCharacterType => _selectedCharacterType.Value;
    public Observable<ECharacterType> SelectedCharacterTypeObservable() => _selectedCharacterType;

    private ReactiveProperty<ECharacterType> _choiceCharacterType;
    public ECharacterType ChoiceCharacterType => _choiceCharacterType.Value;
    public Observable<ECharacterType> ChoiceCharacterTypeObservable() => _choiceCharacterType;

    [field: Header("캐릭터 가짜 데이터")]
    [field: SerializeField] public CharacterData HitData { get; private set; }
    [field: SerializeField] public CharacterData FrostData { get; private set; }
    [field: SerializeField] public CharacterData KaneData { get; private set; }

    public bool IsActive { get; private set; }

    private void Awake()
    {
        ECharacterType currentCharacter = Singleton.Instance<GameManager>().GetCharacter(ActorType.User);
        _selectedCharacterType = new ReactiveProperty<ECharacterType>(currentCharacter);
        _choiceCharacterType = new ReactiveProperty<ECharacterType>(currentCharacter);
    }

    /// <summary>
    /// 선택 캐릭터 타입을 변경합니다.
    /// </summary>
    /// <param name="characterType"></param>
    public void SetSelectedCharacterType(ECharacterType characterType)
    {
        _selectedCharacterType.Value = characterType;
    }

    /// <summary>
    /// 초이스된 캐릭터 타입을 변경합니다.
    /// </summary>
    /// <param name="characterType"></param>
    public void SetChoiceCharacterType(ECharacterType characterType)
    {
        _choiceCharacterType.Value = characterType;
    }

    /// <summary>
    /// 뷰 활성화 상태를 변경합니다.
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetActive(bool active)
    {
        IsActive = active;
    }
}