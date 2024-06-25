using Data;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using FMODPlus;
using R3;
using ReactiveTouchDown;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Singleton;
using UnityEngine.UI;
using Utility;

public class LobbyView : MonoBehaviour
{
    [Header("Character")] [SerializeField] private GameObject hit;
    [SerializeField] private GameObject frost;
    [SerializeField] private GameObject kane;

    [Header("Pet")] [SerializeField] private GameObject scorchwing;
    [SerializeField] private GameObject icebound;
    [SerializeField] private GameObject aerion;

    [Header("Text")] [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI plugText;

    [Header("Profile Image")] [SerializeField]
    private ChangeImage profileImage;

    /// <summary>
    /// 매치 버튼
    /// </summary>
    [Header("Buttons")] [SerializeField] private MeshRenderer matchButton;

    /// <summary>
    /// 캡처 버튼
    /// </summary>
    [SerializeField] private MeshRenderer captureButton;

    /// <summary>
    /// 랭킹 버튼
    /// </summary>
    [SerializeField] private MeshRenderer rankButton;

    [Space] [SerializeField] private Button mailButton;

    /// <summary>
    /// 퀘스트 버튼
    /// </summary>
    [SerializeField] private Button questButton;

    /// <summary>
    /// 캐릭터 버튼
    /// </summary>
    [SerializeField] private Button characterButton;

    /// <summary>
    /// 친구
    /// </summary>
    [SerializeField] private Button friendButton;

    /// <summary>
    /// 인벤토리
    /// </summary>
    [SerializeField] private Button inventoryButton;

    /// <summary>
    /// 설정 버튼
    /// </summary>
    [SerializeField] private Button settingButton;

    /// <summary>
    /// 존재하는 모든 펫을 얻을 수 있는 치트 버튼
    /// </summary>
    [SerializeField] private Button petCheatButton;


    [Tooltip("미구현 메세지")] [SerializeField] private string[] errorMessage;

    [Header("Event")] public UnityEvent OnClickCapture;
    public UnityEvent OnClickMatch;

    [Header("Toast PC")] [SerializeField] private PCToastGenerate toastGenerate;

    [Header("Audio")] [SerializeField] private FMODAudioSource clickAudioSource;

    [Header("Screen FX")] [SerializeField] private MeshRenderer screenRenderer;
    private readonly Color ClickColor = new Color(0f, 0f, 0f, 0.4f);

    // Constants
    private static readonly int DistortionStrength = Shader.PropertyToID("_DistortionStrength");
    private static readonly int ImageBrightness = Shader.PropertyToID("_ImageBrightness");
    private static readonly int DistortionSpeed = Shader.PropertyToID("_Distortion_Speed");

    /// <summary>
    /// 프로필 이미지를 바인딩 합니다.
    /// </summary>
    public void BindProfileImage(ECharacterType characterType)
    {
        profileImage.SetCharacterType(characterType);
    }

    /// <summary>
    /// 토스트 메세지를 생성합니다.
    /// </summary>
    /// <param name="index">에러 메세지 인덱스</param>
    /// <param name="toastType">시간</param>
    public void ShowToastMessage(int index, LGUtility.EToast toastType = LGUtility.EToast.LENGTH_SHORT)
    {
        string message = errorMessage[index];
        toastGenerate.ShowToastMessage(message, toastType);
    }

    /// <summary>
    /// 매치 버튼을 클릭했을 때 옵저버입니다.
    /// </summary>
    /// <returns>MatchButton Observer</returns>
    public Observable<Unit> MatchButtonDownAsObservable()
    {
        return matchButton.gameObject.TouchDownAsObservable();
    }

    /// <summary>
    /// 매치 버튼을 땟을 때 옵저버입니다.
    /// </summary>
    /// <returns>MatchButton Observer</returns>
    public Observable<Unit> MatchButtonUpAsObservable()
    {
        return matchButton.gameObject.TouchUpAsObservable();
    }

    /// <summary>
    /// 캡쳐 버튼 클릭 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> CaptureButtonDownAsObservable()
    {
        return captureButton.gameObject.TouchDownAsObservable();
    }

    /// <summary>
    /// 캡쳐 버튼 땟을 때 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> CaptureButtonUpAsObservable()
    {
        return captureButton.gameObject.TouchUpAsObservable();
    }

    /// <summary>
    /// 랭킹 버튼 클릭 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> RankButtonDownAsObservable()
    {
        return rankButton.gameObject.TouchDownAsObservable();
    }

    /// <summary>
    /// 랭킹 버튼 땟을 때 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> RankButtonUpAsObservable()
    {
        return rankButton.gameObject.TouchUpAsObservable();
    }

    /// <summary>
    /// 메일 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> MailButtonAsObservable()
    {
        return mailButton.OnClickAsObservable();
    }

    /// <summary>
    /// 퀘스트 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> QuestButtonAsObservable()
    {
        return questButton.OnClickAsObservable();
    }

    /// <summary>
    /// 캐릭터 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> CharacterButtonAsObservable()
    {
        return characterButton.OnClickAsObservable();
    }

    /// <summary>
    /// 친구 버튼 옵저버 입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> FriendButtonAsObservable()
    {
        return friendButton.OnClickAsObservable();
    }

    /// <summary>
    /// 인벤토리 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> InventoryButtonAsObservable()
    {
        return inventoryButton.OnClickAsObservable();
    }

    /// <summary>
    /// 세팅 버튼 옵저버 입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> SettingButtonAsObservable()
    {
        return settingButton.OnClickAsObservable();
    }


    public Observable<Unit> CheatButtonAsObservable()
    {
        return petCheatButton.OnClickAsObservable();
    }

    /// <summary>
    /// 미구현 메세지를 띄웁니다.
    /// </summary>
    public void ShowErrorMessage(int index)
    {
        LGUtility.Toast(errorMessage[index]);
    }

    /// <summary>
    /// 닉네임을 설정합니다.
    /// </summary>
    /// <param name="nickName"></param>
    public void SetNickName(string nickName)
    {
        nickNameText.text = nickName;
    }

    /// <summary>
    /// 레벨을 설정합니다.
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    /// <summary>
    /// 코인을 설정합니다.
    /// </summary>
    /// <param name="coin"></param>
    public void SetCoin(uint coin)
    {
        coinText.text = coin.ToString("N0");
    }

    /// <summary>
    /// 플러그를 설정합니다.
    /// </summary>
    /// <param name="plug"></param>
    public void SetPlug(uint plug)
    {
        plugText.text = plug.ToString("N0");
    }

    /// <summary>
    /// 캐릭터를 보이게 합니다.
    /// </summary>
    /// <param name="characterType">보이게할 캐릭터 타입</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void ShowCharacter(ECharacterType characterType)
    {
        hit.SetActive(false);
        frost.SetActive(false);
        kane.SetActive(false);

        switch (characterType)
        {
            case ECharacterType.Hit:
                hit.SetActive(true);
                break;
            case ECharacterType.Frost:
                frost.SetActive(true);
                break;
            case ECharacterType.Kane:
                kane.SetActive(true);
                break;
            case ECharacterType.Storm:
            case ECharacterType.E:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
        }
    }

    /// <summary>
    /// 캐릭터 타입에 알맞는 펫을 활성화 시키는 함수
    /// </summary>
    /// <param name="characterType">펫을 활성화해야 하는 캐릭터의 유형</param>
    public void ActivePet(ECharacterType characterType)
    {
        switch (characterType)
        {
            case ECharacterType.Hit:
                TryPetActive(EPetType.Scorchwing);
                break;
            case ECharacterType.Frost:
                TryPetActive(EPetType.Icebound);
                break;
            case ECharacterType.Kane:
                TryPetActive(EPetType.Aerion);
                break;
            case ECharacterType.Storm:
                TryPetActive(EPetType.Aerion);
                break;
            case ECharacterType.None:
            case ECharacterType.E:
            default:
                throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
        }
    }

    /// <summary>
    /// 해당 펫을 활성화 시킬 수 있다면 활성화를 시키고 그에 대한 결과를 반환합니다.
    /// </summary>
    /// <param name="petType">활성화 시킬 펫</param>
    /// <returns>성공 여부</returns>
    private void TryPetActive(EPetType petType)
    {
        scorchwing.SetActive(false);
        icebound.SetActive(false);
        aerion.SetActive(false);

        bool hasPet = Singleton.Instance<GameManager>().HasPet(petType);
        if (hasPet)
        {
            bool isEnablePet = Singleton.Instance<GameManager>().IsEnablePet(petType);
            if (isEnablePet)
            {
                switch (petType)
                {
                    case EPetType.Scorchwing:
                        scorchwing.SetActive(true);
                        break;
                    case EPetType.Icebound:
                        icebound.SetActive(true);
                        break;
                    case EPetType.Aerion:
                        aerion.SetActive(true);
                        break;
                    case EPetType.Electra:
#if UNITY_EDITOR
                        Debug.LogError("아직 미구현 입니다.");
#endif
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(petType), petType, null);
                }
            }
        }
    }

    /// <summary>
    /// 프로필 이미지를 업데이트 합니다.
    /// </summary>
    public void UpdateProfileImage()
    {
        ECharacterType characterType = Singleton.Instance<GameManager>().GetCharacter(ActorType.User);
        profileImage.SetCharacterType(characterType);
        profileImage.UpdateChangeImage();
    }

    /// <summary>
    /// 클릭 사운드를 재생합니다.
    /// </summary>
    public void PlayClickSound()
    {
        clickAudioSource.Play();
    }

    /// <summary>
    /// 처음에 시작할 때 모니터 연출을 처리합니다.
    /// </summary>
    public void InitScreenFX()
    {
        const float destDistortion = 0f;

        screenRenderer.material.SetFloat(DistortionStrength, 1.35f);
        screenRenderer.material.SetFloat(DistortionSpeed, 2f);
        screenRenderer.material.SetFloat(ImageBrightness, 0f);

        Sequence sequence = DOTween.Sequence();

        var distortionAnimation = DOTween.To(() => screenRenderer.material.GetFloat(DistortionStrength),
                x => screenRenderer.material.SetFloat(DistortionStrength, x),
                destDistortion, 1f)
            .SetEase(Ease.OutBounce);

        var imageBrightnessAnimation = DOTween.To(() => screenRenderer.material.GetFloat(ImageBrightness),
                x => screenRenderer.material.SetFloat(ImageBrightness, x),
                1f, 1f)
            .SetEase(Ease.OutBounce);

        sequence.Append(distortionAnimation);
        sequence.Join(imageBrightnessAnimation);
        sequence.SetAutoKill();
    }

    /// <summary>
    /// 클릭 효과를 재생합니다.
    /// </summary>
    /// <param name="touchTarget"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void PlayClickFX(ETouchTarget touchTarget)
    {
        switch (touchTarget)
        {
            case ETouchTarget.Match:
                matchButton.material.color = Color.clear;
                break;
            case ETouchTarget.Capture:
                captureButton.material.color = Color.clear;
                break;
            case ETouchTarget.Rank:
                rankButton.material.color = Color.clear;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(touchTarget), touchTarget, null);
        }


        DOTween.Kill("TouchFX");

        Tween fade = null;

        switch (touchTarget)
        {
            case ETouchTarget.Match:
                fade = DOTween
                    .To(() => matchButton.material.color, x => matchButton.material.color = x,
                        ClickColor, 0.25f).SetEase(Ease.OutSine);
                break;
            case ETouchTarget.Capture:
                fade = DOTween
                    .To(() => captureButton.material.color, x => captureButton.material.color = x,
                        ClickColor, 0.25f).SetEase(Ease.OutSine);
                break;
            case ETouchTarget.Rank:
                fade = DOTween
                    .To(() => rankButton.material.color, x => rankButton.material.color = x,
                        ClickColor, 0.25f).SetEase(Ease.OutSine);
                break;
        }

        if (fade == null)
            return;

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.SetId("TouchFX");
        fadeSequence.Append(fade);
    }


    public void OnResetClickFX(ETouchTarget touchTarget)
    {
        DOTween.Kill("TouchFX");

        TweenerCore<Color, Color, ColorOptions> fade = null;

        switch (touchTarget)
        {
            case ETouchTarget.Match:
                fade = DOTween
                    .To(() => matchButton.material.color, x => matchButton.material.color = x,
                        Color.clear, 0.25f).SetEase(Ease.OutSine);
                break;
            case ETouchTarget.Capture:
                fade = DOTween
                    .To(() => captureButton.material.color, x => captureButton.material.color = x,
                        Color.clear, 0.25f).SetEase(Ease.OutSine);
                break;
            case ETouchTarget.Rank:
                fade = DOTween
                    .To(() => rankButton.material.color, x => rankButton.material.color = x,
                        Color.clear, 0.25f).SetEase(Ease.OutSine);
                break;
        }

        if (fade == null)
            return;

        Sequence fadeSequence = DOTween.Sequence();
        fadeSequence.SetId("TouchFX");
        fadeSequence.Append(fade);
    }

    public void OnClickButton(ETouchTarget touchTarget)
    {
        PlayClickSound();
        switch (touchTarget)
        {
            case ETouchTarget.Match:
                Singleton.Instance<GameManager>().RandomChoiceAI();
                OnClickMatch?.Invoke();
                break;
            case ETouchTarget.Capture:
                
#if UNITY_STANDALONE || UNITY_EDITOR
                ShowToastMessage(5);
#else
                OnClickCapture?.Invoke();
#endif
                break;
            case ETouchTarget.Rank:
#if UNITY_STANDALONE || UNITY_EDITOR
                ShowToastMessage(0);
#else
                ShowErrorMessage(0);
#endif
                break;
        }
    }
}