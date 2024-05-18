using Data;
using R3;
using UnityEngine;
using USingleton;

[RequireComponent(typeof(BattleModel))]
[RequireComponent(typeof(BattleView))]
public class BattlePresenter : MonoBehaviour
{
    private BattleModel _battleModel;
    private BattleView _battleView;

    private GameManager CurrentGameManager => Singleton.Instance<GameManager>();

    private void Start()
    {
        _battleModel = GetComponent<BattleModel>();
        _battleView = GetComponent<BattleView>();

        #region User

        // 유저 이름을 설정합니다.
        string userName = CurrentGameManager.GetNickname();
        _battleView.SetUserNameText(ActorType.User, userName);

        // 유저 프로필 이미지를 설정합니다.
        ECharacterType userCharacter = CurrentGameManager.GetCharacter(ActorType.User);
        _battleView.SetCharacterImage(ActorType.User, userCharacter);

        // 유저 정령 UI를 설정한다.
        Sprite userPatImage = CurrentGameManager.GetPat(ActorType.User).GetProfileImage();
        _battleView.SetPatUI(ActorType.User, userPatImage);

        #endregion

        #region AI

        // AI 프로필 이미지를 설정합니다.
        ECharacterType aiCharacter = CurrentGameManager.GetCharacter(ActorType.AI);
        _battleView.SetCharacterImage(ActorType.AI, aiCharacter);
        
        // AI 정령 UI를 설정한다.
        Sprite aiPatImage = CurrentGameManager.GetPat(ActorType.AI).GetProfileImage();
        _battleView.SetPatUI(ActorType.AI, aiPatImage);

        #endregion

        // 로딩 텍스트 렌더링 옵저버
        _battleView.UpdateLoadingTextObservable()
            .Subscribe(_ => _battleView.UpdateLoadingText()).AddTo(this);
    }
}