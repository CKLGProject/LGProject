using Data;
using R3;
using UnityEngine;
using UnityEngine.Singleton;

[RequireComponent(typeof(LoadingView))]
public class LoadingPresenter : MonoBehaviour
{
    private LoadingView _loadingView;

    private GameManager GameManager => Singleton.Instance<GameManager>();

    private void Start()
    {
        _loadingView = GetComponent<LoadingView>();

        #region User

        // 유저 이름을 설정합니다.
        string userName = GameManager.GetNickname();
        _loadingView.SetUserNameText(ActorType.User, userName);

        // 유저 프로필 이미지를 설정합니다.
        ECharacterType userCharacter = GameManager.GetCharacter(ActorType.User);
        _loadingView.ShowCharacterProfile(ActorType.User, userCharacter);
        
        // 유저 정령 UI를 설정한다.
        EPetType userPetType = GameManager.GetPetType(ActorType.User);

        bool canUseUserPetUIActive = userPetType != EPetType.None;
        _loadingView.SetActivePetGroup(ActorType.User, canUseUserPetUIActive);
        _loadingView.ShowPetProfile(ActorType.User, userPetType);

        #endregion

        #region AI

        // AI 프로필 이미지를 설정합니다.
        ECharacterType aiCharacter = GameManager.GetCharacter(ActorType.AI);
        _loadingView.ShowCharacterProfile(ActorType.AI, aiCharacter);
        
        // AI 정령 UI를 설정한다.
        EPetType aiPetType = GameManager.GetPetType(ActorType.AI);

        bool canUseAIPetUIActive = aiPetType != EPetType.None;
        _loadingView.SetActivePetGroup(ActorType.AI, canUseAIPetUIActive);
        _loadingView.ShowPetProfile(ActorType.AI, aiPetType);

        #endregion

        // 로딩 텍스트 렌더링 옵저버
        _loadingView.UpdateLoadingTextObservable()
            .Subscribe(_ => _loadingView.UpdateLoadingText()).AddTo(this);
    }
}