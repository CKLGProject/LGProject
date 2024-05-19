using Data;
using R3;
using System;
using UnityEngine;

public class BattleModel : MonoBehaviour
{
    // 카운트 다운 값
    public const int CountMax = 3;

    private SerializableReactiveProperty<int> _userHealthProperty;
    private SerializableReactiveProperty<int> _aiHealthProperty;
    private SerializableReactiveProperty<int> _countDownProperty;

    private void Awake()
    {
        _countDownProperty = new SerializableReactiveProperty<int>(CountMax);
    }

    /// <summary>
    /// 체력을 초기화 합니다.
    /// </summary>
    /// <param name="actorType"></param>
    /// <param name="lifePoint"></param>
    public void InitHealth(ActorType actorType, int lifePoint)
    {
        switch (actorType)
        {
            case ActorType.User:
                _userHealthProperty = new SerializableReactiveProperty<int>(lifePoint);
                break;
            case ActorType.AI:
                _aiHealthProperty = new SerializableReactiveProperty<int>(lifePoint);
                break;
        }
    }

    /// <summary>
    /// 체력을 싱크하는 함수
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="lifePoint">라이프 포인트</param>
    public void SyncHealth(ActorType actorType, int lifePoint)
    {
        switch (actorType)
        {
            case ActorType.User:
                _userHealthProperty.Value = lifePoint;
                break;
            case ActorType.AI:
                _aiHealthProperty.Value = lifePoint;
                break;
        }
    }

    /// <summary>
    /// 카운트 다운 값을 value로 지정합니다.
    /// </summary>
    /// <param name="value"></param>
    public void SetCountdown(int value)
    {
        _countDownProperty.Value = value;
    }

    public Observable<int> UserHealthObservable => _userHealthProperty.AsObservable();
    public Observable<int> AIHealthObservable => _aiHealthProperty.AsObservable();
    public Observable<int> CountDownObservable => _countDownProperty.AsObservable();
}