using Data;
using R3;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class LoadingView : MonoBehaviour
{
    [Serializable]
    public class CardUI
    {
        public TextMeshProUGUI NameText;
        public GameObject[] CharacterList;
        public GameObject[] PetList;
        public GameObject PetGroup;
    }

    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("User")] [SerializeField] private CardUI userCardUI;

    [Header("AI")] [SerializeField] private CardUI aiCardUI;

    [SerializeField] private CharacterImageData[] characterImageDataList;


    private int _dotCount;

    /// <summary>
    /// 유저 정령 프로필 이미지를 value로 변경합니다.
    /// </summary>
    /// <param name="target">타겟 액터</param>
    /// <param name="characterType">캐릭터 타입</param>
    public void ShowCharacterProfile(ActorType target, ECharacterType characterType)
    {
        switch (target)
        {
            case ActorType.User:
                foreach (GameObject characterObject in userCardUI.CharacterList)
                    characterObject.SetActive(false);

                userCardUI.CharacterList[(int)characterType].SetActive(true);
                break;
            case ActorType.AI:
                foreach (GameObject characterObject in aiCardUI.CharacterList)
                    characterObject.SetActive(false);

                aiCardUI.CharacterList[(int)characterType].SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 로딩 텍스트를 업데이트 합니다.
    /// </summary>
    public void UpdateLoadingText()
    {
        // 호출될때마다 점 개수에 맞춰 렌더링을 하면 된다.

        _dotCount += 1;

        if (_dotCount > 3)
            _dotCount = 0;

        StringBuilder stringBuilder = new();
        stringBuilder.Append("Loading");

        for (int i = 0; i < _dotCount; i++)
            stringBuilder.Append(".");

        loadingText.text = stringBuilder.ToString();
    }

    /// <summary>
    /// 유저 이름 텍스트를 value로 설정합니다.
    /// </summary>
    public void SetUserNameText(ActorType target, string value)
    {
        switch (target)
        {
            case ActorType.User:
                if (!userCardUI.NameText)
                    return;
                userCardUI.NameText.text = value;
                break;
            case ActorType.AI:
                if (!aiCardUI.NameText)
                    return;
                aiCardUI.NameText.text = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 펫 이름 텍스트를 value로 설정합니다.
    /// </summary>
    /// <param name="target">타겟 액터</param>
    /// <param name="petType">펫 타입</param>
    public void ShowPetProfile(ActorType target, EPetType petType)
    {
        switch (target)
        {
            case ActorType.User:
                foreach (GameObject petObject in userCardUI.PetList)
                    petObject.SetActive(false);

                userCardUI.PetList[(int)petType].SetActive(true);
                break;
            case ActorType.AI:
                foreach (GameObject petObject in aiCardUI.PetList)
                    petObject.SetActive(false);

                aiCardUI.PetList[(int)petType].SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 해당 액터 타겟의 펫 그룹 활성화 상태를 설정합니다.
    /// </summary>
    /// <param name="target">액터 타겟</param>
    /// <param name="active">활성화 여부</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetActivePetGroup(ActorType target, bool active)
    {
        switch (target)
        {
            case ActorType.User:
                userCardUI.PetGroup.SetActive(active);
                break;
            case ActorType.AI:
                aiCardUI.PetGroup.SetActive(active);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// Loading Text 업데이트 옵저버
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> UpdateLoadingTextObservable()
    {
        return Observable.Interval(TimeSpan.FromSeconds(0.25f));
    }
}