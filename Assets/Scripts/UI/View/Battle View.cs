using Data;
using R3;
using System;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleView : MonoBehaviour
{
    public enum Target
    {
        None,
        User,
        AI
    }

    [Serializable]
    public class CardUI
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI PatNameText;
        public Image CharacterImage;
        public Image PatImage;
    }

    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("User")] [SerializeField] private CardUI userCardUI;


    [Header("AI")] [SerializeField] private CardUI aiCardUI;

    [SerializeField] private CharacterImageData[] characterImageDataList;
    [SerializeField] private PatData[] patDataList;

    private int _dotCount;

    /// <summary>
    /// 유저 정령 프로필 이미지를 value로 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void SetCharacterProfileImage(Target target, Sprite value)
    {
        if (!userCardUI.CharacterImage)
            return;

        switch (target)
        {
            case Target.User:
                userCardUI.CharacterImage.sprite = value;
                break;
            case Target.AI:
                aiCardUI.CharacterImage.sprite = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 유저 정령 프로필 이미지를 value로 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void SetUserProfileImage(Target target, ECharacterType value)
    {
        if (!userCardUI.PatImage)
            return;

        switch (target)
        {
            case Target.User:
                userCardUI.PatImage.sprite = CalculateCharacterImage(value);
                break;
            case Target.AI:
                aiCardUI.PatImage.sprite = CalculateCharacterImage(value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 유저 정령 프로필 이미지를 value로 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void SetUserProfileImage(Target target, Sprite value)
    {
        if (!userCardUI.PatImage)
            return;

        switch (target)
        {
            case Target.User:
                userCardUI.PatImage.sprite = value;
                break;
            case Target.AI:
                aiCardUI.PatImage.sprite = value;
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
    public void SetUserNameText(Target target, string value)
    {
        if (!userCardUI.NameText)
            return;

        switch (target)
        {
            case Target.User:
                userCardUI.NameText.text = value;
                break;
            case Target.AI:
                aiCardUI.NameText.text = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(target), target, null);
        }
    }

    /// <summary>
    /// 펫 이름 텍스트를 value로 설정합니다.
    /// </summary>
    public void SetPatUI(Target target, EPatType value)
    {
        if (!userCardUI.PatNameText)
            return;

        switch (target)
        {
            case Target.User:
                userCardUI.PatNameText.text = CalculatePatName(value);
                userCardUI.PatImage.sprite =
                break;
            case Target.AI:
                aiCardUI.PatNameText.text = CalculatePatName(value);;
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

    /// <summary>
    /// characterType에 알맞는 캐릭터 이미지를 반환합니다.
    /// </summary>
    /// <param name="characterType">캐릭터 타입</param>
    /// <returns>캐릭터 이미지</returns>
    private Sprite CalculateCharacterImage(ECharacterType characterType)
    {
        return (from data in characterImageDataList
                where data.CharacterType == characterType
                select data.CharacterProfileImage)
            .FirstOrDefault();
    }

    /// <summary>
    /// patType에 알맞는 캐릭터 이름을 반환합니다.
    /// </summary>
    /// <param name="patType">정령 타입</param>
    /// <returns>캐릭터 이미지</returns>
    private string CalculatePatName(EPatType patType)
    {
        return (from data in patDataList
                where data.PatType == patType
                select data.PatName)
            .FirstOrDefault();
    }
}