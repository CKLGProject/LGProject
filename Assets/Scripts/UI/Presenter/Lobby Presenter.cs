using R3;
using UnityEngine;

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

        _lobbyView.MatchButtonAsObservable()
            .Subscribe(OnClickMatchButton);

        _lobbyView.RecruitButtonAsObservable()
            .Subscribe(OnClickRecruitButton);

        _lobbyView.RankButtonAsObservable()
            .Subscribe(OnClickRankButton);
    }

    private void OnClickRankButton(Unit obj)
    {
        Debug.Log("랭킹 이동");
    }

    private void OnClickRecruitButton(Unit obj)
    {
        Debug.Log("모집 이동");
    }

    private void OnClickMatchButton(Unit obj)
    {
        Debug.Log("매치 이동");
    }
}