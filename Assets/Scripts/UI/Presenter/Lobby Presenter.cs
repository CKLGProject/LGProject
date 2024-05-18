using R3;
using UnityEngine;
using USingleton;

[RequireComponent(typeof(LobbyView))]
[RequireComponent(typeof(LobbyModel))]
public class LobbyPresenter : MonoBehaviour
{
    private LobbyView _lobbyView;
    private LobbyModel _lobbyModel;

    private void Start()
    {
        _lobbyView = GetComponent<LobbyView>();
        _lobbyModel = GetComponent<LobbyModel>();
        
        // Model
        _lobbyModel.NicknameObservable
            .Subscribe(nickName => _lobbyView.SetNickName(nickName));
        
        _lobbyModel.LevelObservable
            .Subscribe(level => _lobbyView.SetLevel(level));
        
        _lobbyModel.CoinObservable
            .Subscribe(coin => _lobbyView.SetCoin(coin));
        
        _lobbyModel.PlugObservable
            .Subscribe(plug => _lobbyView.SetPlug(plug));

        // View
        _lobbyView.MatchButtonAsObservable()
            .Subscribe(OnClickMatchButton);

        _lobbyView.RankButtonAsObservable()
            .Subscribe(OnClickRankButton);

        _lobbyView.MailButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage());
        
        _lobbyView.QuestButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage());
        
        _lobbyView.CharacterButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage());
        
        _lobbyView.FriendButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage());
        
        _lobbyView.SettingButtonAsObservable()
            .Subscribe(_ => _lobbyView.ShowErrorMessage());
        
        _lobbyModel.Nickname = PlayerPrefs.GetString("Nickname", "Guest");
        _lobbyModel.Level = PlayerPrefs.GetInt("Level", 0);
        _lobbyModel.Coin = (uint)PlayerPrefs.GetInt("Coin", 0);
        _lobbyModel.Plug = (uint)PlayerPrefs.GetInt("Plug", 0);
    }

    private void OnClickRankButton(Unit obj)
    {
       _lobbyView.OnClickRanking?.Invoke();
    }

    private void OnClickMatchButton(Unit obj)
    {
        Singleton.Instance<GameManager>().RandomChoiceAI();
       _lobbyView.OnClickMatch?.Invoke();
    }
}