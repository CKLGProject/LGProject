using Data;
using LGProject;
using R3;
using R3.Triggers;
using ReactiveCountdown;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USingleton;

[RequireComponent(typeof(BattleView))]
[RequireComponent(typeof(BattleModel))]
public class BattlePresenter : MonoBehaviour
{
    private BattleView _battleView;
    private BattleModel _battleModel;

    private GameManager CurrentGameManager => Singleton.Instance<GameManager>();

    private void Start()
    {
        _battleView = GetComponent<BattleView>();
        _battleModel = GetComponent<BattleModel>();

        // 이름 설정
        string nickName = CurrentGameManager.GetNickname();
        _battleView.SetNameText(ActorType.User, nickName);
        _battleView.SetNameText(ActorType.AI, "AI");

        // 뷰를 전부 안보이도록 처리
        _battleView.AllHideView();

        // 유저의 생명 포인트에 따라 UI를 업데이트하는 옵저버
        _battleModel.UserHealthObservable
            .Subscribe(lifePoint => _battleView.UpdateLifPointUI(ActorType.User, lifePoint))
            .AddTo(this);

        // AI의 생명 포인트에 따라 UI를 업데이트하는 옵저버
        _battleModel.AIHealthObservable
            .Subscribe(lifePoint => _battleView.UpdateLifPointUI(ActorType.AI, lifePoint))
            .AddTo(this);

        // 유저의 생명이 0이 되었을 때 Lose Popup을 띄우는 옵저버
        _battleModel.UserHealthObservable
            .Where(lifePoint => lifePoint == 0)
            .Subscribe(_ => _battleView.ShowLosePopup())
            .AddTo(this);

        // AI의 생명이 0이 되었을 때 Wid Popup을 띄우는 옵저버
        _battleModel.AIHealthObservable
            .Where(lifePoint => lifePoint == 0)
            .Subscribe(_ => _battleView.ShowWinPopup())
            .AddTo(this);

        // 유저의 데미지 게이지를 갱신하는 옵저버
        _battleModel.UserDamageGageObservable
            .Subscribe(damageGage => _battleView.UpdateDamageGageUI(ActorType.User, damageGage))
            .AddTo(this);
        
        // AI의 데미지 게이지를 갱신하는 옵저버
        _battleModel.AIDamageGageObservable
            .Subscribe(damageGage => _battleView.UpdateDamageGageUI(ActorType.AI, damageGage))
            .AddTo(this);

        // 카운트 다운 옵저버
        _battleModel.CountDownObservable
            .Subscribe(count => _battleView.UpdateCountDownUI(count))
            .AddTo(this);

        // 카운트 다운 옵저버
        this.CountdownAsObservable(BattleModel.CountMax)
            .Subscribe(
                count => _battleModel.SetCountdown(count),
                _ => BattleSceneManager.Instance.GameStart())
            .AddTo(this);
    }
}