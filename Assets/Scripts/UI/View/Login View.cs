using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.UI;
using USingleton;
using Utility;

public class LoginView : MonoBehaviour
{
    public Container LoginViewContainer;
    public TMP_InputField NicknameInputField;
    public Button LoginButton;
    public SceneLoader LobbySceneLoader;
    
    // Constants
    private const int SuccessfulFirstLogin = 1;

    /// <summary>
    /// 로비 씬으로 연결합니다.
    /// </summary>
    /// <param name="useInputField">연결하면서 입력필드의 닉네임을 사용할 것인가?</param>
    public void Connect(bool useInputField = false)
    {
        if (useInputField)
        {
            PlayerPrefs.SetInt("IsFirstLogin", SuccessfulFirstLogin);
            PlayerPrefs.SetString("Nickname", NicknameInputField.text);
            Singleton.Instance<GameManager>().SetNickname(NicknameInputField.text);
        }
        //TODO:GameManager Start에서 기본으로 처리됩니다.
        // else
        // {
        //     string nickName = PlayerPrefs.GetString("Nickname","Guest");
        //     Singleton.Instance<GameManager>().SetNickname(nickName);
        // }

        LobbySceneLoader.gameObject.SetActive(true);
    }

    /// <summary>
    /// 입력 필드가 입력되어있는지 체크합니다.
    /// </summary>
    /// <returns>입력되어있으면 true를 반환합니다.</returns>
    public bool IsNicknameNotEmpty()
    {
        return !string.IsNullOrEmpty(NicknameInputField.text);
    }

    /// <summary>
    /// 로그인 뷰를 활성화 상태로 만듭니다.
    /// </summary>
    public void ShowLoginView()
    {
        LoginViewContainer.PositionReset();
    }
}