using R3;
using UnityEngine;
using USingleton;

[RequireComponent(typeof(LoginView))]
public class LoginPresenter : MonoBehaviour
{
    private LoginView _view;

    private void Start()
    {
        _view = GetComponent<LoginView>();

        if (IsFirstLogin())
            _view.SetActive(true);
        else
            _view.Connect();

        _view.LoginButton.onClick
            .AsObservable()
            .Where(_ => _view.IsNicknameNotEmpty())
            .Subscribe(_ => _view.Connect(true))
            .AddTo(this);
    }

    /// <summary>
    /// 처음 로그인한다면 true를 반환합니다.
    /// </summary>
    /// <returns></returns>
    private bool IsFirstLogin()
    {
        return PlayerPrefs.GetInt("IsFirstLogin", 0) == 0;
    }
}