using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.UI;
using USingleton;

public class LoginView : MonoBehaviour
{
    public enum MoveScene
    {
        Lobby,
        ChooseCharacter
    }

    public GameObject LoginViewContainer;
    public TMP_InputField NicknameInputField;
    public Button LoginButton;
    public SceneLoader LobbySceneLoader;
    public SceneLoader ChooseCharacterSceneLoader;
    
    // Constants
    private const int SuccessfulFirstLogin = 1;

    /// <summary>
    /// 로비 씬으로 연결합니다.
    /// </summary>
    /// <param name="moveScene">이동할 씬</param>
    /// <param name="useInputField">연결하면서 입력필드의 닉네임을 사용할 것인가?</param>
    public void Connect(MoveScene moveScene, bool useInputField = false)
    {
        if (useInputField)
        {
            PlayerPrefs.SetInt("IsFirstLogin", SuccessfulFirstLogin);
            PlayerPrefs.SetString("Nickname", NicknameInputField.text);
            Singleton.Instance<GameManager>().Nickname = NicknameInputField.text;
        }
        else
            Singleton.Instance<GameManager>().Nickname = PlayerPrefs.GetString("Nickname");

        switch (moveScene)
        {
            case MoveScene.Lobby:
                LobbySceneLoader.gameObject.SetActive(true);
                break;
            case MoveScene.ChooseCharacter:
                ChooseCharacterSceneLoader.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(moveScene), moveScene, null);
        }
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
    /// 로그인 뷰의 활성화 상태를 설정합니다.
    /// </summary>
    /// <param name="isActive">활성화 상태</param>
    public void SetActive(bool isActive)
    {
        LoginViewContainer.SetActive(isActive);
    }
}