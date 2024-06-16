using Data;
using R3;
using UnityEngine;
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
        
        _lobbyView.MatchButtonAsObservable()
            .Where(_=> !_lobbyPopupModel.IsActive)
            .Subscribe(OnClickMatchButton);

        _lobbyView.RankButtonAsObservable()
            .Where(_=> !_lobbyPopupModel.IsActive)
            .Subscribe(_ => _lobbyView.ShowErrorMessage(0));
        
        _lobbyView.CaptureButtonAsObservable()
            .Where(_=> !_lobbyPopupModel.IsActive)
            .Subscribe(OnClickCaptureButton);

        _lobbyView.MailButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(0));
        
        _lobbyView.QuestButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(0));
        
        _lobbyView.CharacterButtonAsObservable()
            .Subscribe(_ => _lobbyPopupView.SetActivePopupView(true));
        
        _lobbyView.FriendButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(0));
        
        _lobbyView.SettingButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage(0));
        
        _lobbyModel.Nickname = PlayerPrefs.GetString("Nickname", "Guest");
        _lobbyModel.Level = PlayerPrefs.GetInt("Level", 0);
        _lobbyModel.Coin = (uint)PlayerPrefs.GetInt("Coin", 0);
        _lobbyModel.Plug = (uint)PlayerPrefs.GetInt("Plug", 0);
        
        // 임시 값
        _lobbyModel.Coin = 1958;
        _lobbyModel.Plug = 3270;
    }

    private void OnClickCaptureButton(Unit obj)
    {
       _lobbyView.OnClickCapture?.Invoke();
    }

    private void OnClickMatchButton(Unit obj)
    {
        Singleton.Instance<GameManager>().RandomChoiceAI();
       _lobbyView.OnClickMatch?.Invoke();
    }
}