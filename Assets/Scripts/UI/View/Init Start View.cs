using TMPro;
using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.Singleton;
using Utility;

public class InitStartView : MonoBehaviour
{
    [SerializeField] private SceneLoader lobbySceneLoader;
    [SerializeField] private TextMeshProUGUI touchText;

    private const string MobileText = "화면을 터치해주세요!";
    private const string PCText = "아무 키나 누르세요!";

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
        string nickName = PlayerPrefs.GetString("Nickname", "Guest");
        Singleton.Instance<GameManager>().SetNickname(nickName);

        lobbySceneLoader.AllowCompletion();
    }


}