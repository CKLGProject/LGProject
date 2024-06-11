using Data;
using R3;
using UnityEngine;
using UnityEngine.Singleton;

[RequireComponent(typeof(LoadingView))]
public class LoadingPresenter : MonoBehaviour
{
    private LoadingView _loadingView;

    private GameManager CurrentGameManager => Singleton.Instance<GameManager>();

    private void Start()
    {
        _loadingView = GetComponent<LoadingView>();

        #region User

        // 유저 이름을 설정합니다.
        string userName = CurrentGameManager.GetNickname();
        _loadingView.SetUserNameText(ActorType.User, userName);

        // 유저 프로필 이미지를 설정합니다.
        ECharacterType userCharacter = CurrentGameManager.GetCharacter(ActorType.User);
        _loadingView.SetCharacterImage(ActorType.User, userCharacter);

        // 유저 정령 UI를 설정한다.
        Sprite userPatImage = CurrentGameManager.GetPat(ActorType.User).GetProfileImage();
        _loadingView.SetPatUI(ActorType.User, userPatImage);

        #endregion

        #region AI

        // AI 프로필 이미지를 설정합니다.
        ECharacterType aiCharacter = CurrentGameManager.GetCharacter(ActorType.AI);
        _loadingView.SetCharacterImage(ActorType.AI, aiCharacter);
        
        // AI 정령 UI를 설정한다.
        Sprite aiPatImage = CurrentGameManager.GetPat(ActorType.AI).GetProfileImage();
        _loadingView.SetPatUI(ActorType.AI, aiPatImage);

        #endregion

        // 로딩 텍스트 렌더링 옵저버
        _loadingView.UpdateLoadingTextObservable()
            .Subscribe(_ => _loadingView.UpdateLoadingText()).AddTo(this);
    }
}