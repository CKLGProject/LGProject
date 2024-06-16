using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.Singleton;
using Utility;

public class InitStartView : MonoBehaviour
{
    [SerializeField] private SceneLoader lobbySceneLoader;
    [SerializeField] private TextMeshProUGUI touchText;
    [SerializeField] private CanvasGroup screenFX;

    private const string MobileText = "화면을 터치해주세요!";
    private const string PCText = "아무 키나 누르세요!";

    /// <summary>
    /// 페이드 아웃을 실행합니다.
    /// </summary>
    public void PlayFadeOut()
    {
        screenFX.alpha = 1;
        screenFX.DOFade(0, 0.5f).SetDelay(0.5f).SetEase(Ease.InOutSine)
            .OnComplete(() => screenFX.gameObject.SetActive(false));
    }

    /// <summary>
    /// OS에 맞춰 텍스트를 변경합니다.
    /// </summary>
    public void ChangeTextByOS()
    {
        bool isMobile = LGUtility.IsMobile();
        touchText.text = isMobile ? MobileText : PCText;
    }

    /// <summary>
    /// 로비 씬으로 연결합니다.
    /// </summary>
    public void Connect()
    {
        const string nickName = "Guest";
        PlayerPrefs.SetString("Nickname", nickName);
        Singleton.Instance<GameManager>().SetNickname(nickName);

        lobbySceneLoader.AllowCompletion();
    }
}