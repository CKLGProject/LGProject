using Data;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Singleton;

[RequireComponent(typeof(LobbyView))]
[RequireComponent(typeof(LobbyModel))]
public class LobbyPresenter : MonoBehaviour
{
    private LobbyPopupView _lobbyPopupView;
    private LobbyPopupModel _lobbyPopupModel;

    private LobbyView _lobbyView;
    private LobbyModel _lobbyModel;


    private void Start()
    {
        _lobbyView = GetComponent<LobbyView>();
        _lobbyPopupView = FindAnyObjectByType<LobbyPopupView>();

        _lobbyModel = GetComponent<LobbyModel>();
        _lobbyPopupModel = FindAnyObjectByType<LobbyPopupModel>();

        _lobbyView.InitScreenFX();

        // Model
        _lobbyPopupModel.ChoiceCharacterTypeObservable()
            .Subscribe(characterType =>
            {
                // 캐릭터를 보이게 합니다.
                _lobbyView.ShowCharacter(characterType);

                // 펫을 보이게 합니다.
                ECharacterType selectedCharacter = _lobbyPopupModel.SelectedCharacterType;
                _lobbyView.ActivePet(selectedCharacter);
            });

        _lobbyModel.NicknameObservable
            .Subscribe(nickName => _lobbyView.SetNickName(nickName));

        _lobbyModel.LevelObservable
            .Subscribe(level => _lobbyView.SetLevel(level));

        _lobbyModel.CoinObservable
            .Subscribe(coin => _lobbyView.SetCoin(coin));

        _lobbyModel.PlugObservable
            .Subscribe(plug => _lobbyView.SetPlug(plug));

        // View
        ECharacterType selectedCharacter = _lobbyPopupModel.SelectedCharacterType;
        _lobbyView.ActivePet(selectedCharacter);
        _lobbyView.BindProfileImage(selectedCharacter);

        _lobbyView.MatchButtonDownAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Subscribe(_ =>
            {
                ETouchTarget touchTarget = ETouchTarget.Match;
                _lobbyModel.TouchTarget = touchTarget;

                _lobbyView.PlayClickFX(touchTarget);
            });

        _lobbyView.MatchButtonUpAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Where(_ => _lobbyModel.TouchTarget == ETouchTarget.Match)
            .Subscribe(_ => _lobbyView.OnClickButton(ETouchTarget.Match));

        _lobbyView.RankButtonDownAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Subscribe(_ =>
            {
                ETouchTarget touchTarget = ETouchTarget.Rank;
                _lobbyModel.TouchTarget = touchTarget;

                _lobbyView.PlayClickFX(touchTarget);
            });

        _lobbyView.RankButtonUpAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Where(_ => _lobbyModel.TouchTarget == ETouchTarget.Rank)
            .Subscribe(_ => _lobbyView.OnClickButton(ETouchTarget.Rank));

        _lobbyView.CaptureButtonDownAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Subscribe(_ =>
            {
                ETouchTarget touchTarget = ETouchTarget.Capture;
                _lobbyModel.TouchTarget = touchTarget;

                _lobbyView.PlayClickFX(touchTarget);
            });

        _lobbyView.CaptureButtonUpAsObservable()
            .Where(_ => !_lobbyPopupModel.IsActive)
            .Where(_ => _lobbyModel.TouchTarget == ETouchTarget.Capture)
            .Subscribe(_ => _lobbyView.OnClickButton(ETouchTarget.Capture));

        InputSystem.actions.FindActionMap("System").FindAction("Touch").canceled += OnResetClickFX;

        _lobbyView.MailButtonAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyView.InventoryButtonAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyView.QuestButtonAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyView.CharacterButtonAsObservable()
            .Subscribe(_ => _lobbyPopupView.SetActivePopupView(true));

        _lobbyView.FriendButtonAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyView.SettingButtonAsObservable()
            .Subscribe(_ =>
            {
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyView.CheatButtonAsObservable()
            .Subscribe(_ =>
            {
                Singleton.Instance<GameManager>().AddPet(EPetType.Aerion);
                Singleton.Instance<GameManager>().AddPet(EPetType.Electra);
                Singleton.Instance<GameManager>().AddPet(EPetType.Icebound);
                Singleton.Instance<GameManager>().AddPet(EPetType.Scorchwing);
#if UNITY_STANDALONE
                _lobbyView.ShowToastMessage(0);
#else
                _lobbyView.ShowErrorMessage(0);
#endif
            });

        _lobbyModel.Nickname = PlayerPrefs.GetString("Nickname", "Guest");
        _lobbyModel.Level = PlayerPrefs.GetInt("Level", 0);
        _lobbyModel.Coin = (uint)PlayerPrefs.GetInt("Coin", 0);
        _lobbyModel.Plug = (uint)PlayerPrefs.GetInt("Plug", 0);

        // 임시 값
        _lobbyModel.Coin = 1958;
        _lobbyModel.Plug = 3270;
    }

    private void OnResetClickFX(InputAction.CallbackContext obj)
    {
        ETouchTarget touchTarget = _lobbyModel.TouchTarget;
        _lobbyView.OnResetClickFX(touchTarget);

        _lobbyModel.TouchTarget = ETouchTarget.None;
    }
}