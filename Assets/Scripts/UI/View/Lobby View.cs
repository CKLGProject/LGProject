using R3;
using ReactiveTouchDown;
using System;
using UnityEngine;

public class LobbyView : MonoBehaviour
{
    /// <summary>
    /// 매치 버튼
    /// </summary>
    [SerializeField] private GameObject matchButton;

    /// <summary>
    /// 모집 버튼
    /// </summary>
    [SerializeField] private GameObject recruitButton;

    /// <summary>
    /// 랭킹 버튼
    /// </summary>
    [SerializeField] private GameObject rankButton;

    /// <summary>
    /// 매치 버튼 옵저버입니다.
    /// </summary>
    /// <returns>MatchButton Observer</returns>
    public Observable<Unit> MatchButtonAsObservable()
    {
        return matchButton.TouchDownAsObservable();
    }

    /// <summary>
    /// 모집 버튼 옵저버입니다.
    /// </summary>
    /// <returns>RecruitButton Observer</returns>
    public Observable<Unit> RecruitButtonAsObservable()
    {
        return recruitButton.TouchDownAsObservable();
    }

    /// <summary>
    /// 랭킹 버튼 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> RankButtonAsObservable()
    {
        return rankButton.TouchDownAsObservable();
    }
}