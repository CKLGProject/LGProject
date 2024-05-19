using Data;
using R3;
using System;
using UnityEngine;

public class BattleModel : MonoBehaviour
{
    // 카운트 다운 값
    public const int CountMax = 3;

    private readonly SerializableReactiveProperty<int> _userHealthProperty = new(3);
    private readonly SerializableReactiveProperty<int> _aiHealthProperty = new(3);
    private SerializableReactiveProperty<int> _countDownProperty;
    private readonly SerializableReactiveProperty<float> _userDamageGageProperty = new ();
    private readonly SerializableReactiveProperty<float> _aiDamageGageProperty = new ();
    private readonly SerializableReactiveProperty<float> _userUltimateEnergyProperty = new ();
    private readonly SerializableReactiveProperty<float> _aiUltimateEnergyProperty = new ();
    

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
                _userHealthProperty.Value = lifePoint;
                break;
            case ActorType.AI:
                _aiHealthProperty.Value = lifePoint;
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
    /// Ultimate Energy를 싱크하는 함수
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="value"></param>
    public void SyncUltimateEnergy(ActorType actorType, float value)
    {
        switch (actorType)
        {
            case ActorType.User:
                _userUltimateEnergyProperty.Value = value;
                break;
            case ActorType.AI:
                _aiUltimateEnergyProperty.Value = value;
                break;
        }
    }

    /// <summary>
    /// 데미지 게이지를 싱크하는 함수
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="damageGage">데미지 게이지</param>
    public void SyncDamageGage(ActorType actorType,float damageGage)
    {
        switch (actorType)
        {
            case ActorType.User:
                _userDamageGageProperty.Value = damageGage;
                break;
            case ActorType.AI:
                _aiDamageGageProperty.Value = damageGage;
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
    public Observable<float> UserDamageGageObservable => _userDamageGageProperty.AsObservable();
    public Observable<float> AIDamageGageObservable => _aiDamageGageProperty.AsObservable();
    public Observable<float> UserUltimateEnergyObservable => _userUltimateEnergyProperty.AsObservable();
    public Observable<float> AIUltimateEnergyObservable => _aiUltimateEnergyProperty.AsObservable();
}