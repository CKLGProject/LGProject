using R3;
using System;
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

    // 유저 프로필 이미지
    [SerializeField] private Image aiProfileImage;


    private int _dotCount;

    /// <summary>
    /// 유저 정령 프로필 이미지를 value로 변경합니다.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="value"></param>
    public void SetCharacterProfileImage(Target target ,Sprite value)
    {
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
    public void SetUserProfileImage(Target target ,Sprite value)
    {
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
    public void SetPatNameText(Target target, string value)
    {
        switch (target)
        {
            case Target.User:
                userCardUI.PatNameText.text = value;
                break;
            case Target.AI:
                aiCardUI.PatNameText.text = value;
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