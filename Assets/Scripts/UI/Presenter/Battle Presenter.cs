using Cysharp.Threading.Tasks;
using Data;
using LGProject;
using R3;
using R3.Triggers;
using ReactiveCountdown;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Singleton;

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
            .Subscribe(_ =>
            {
                BattleSceneManager.Instance.GameEnd();
                _battleView.ShowLosePopup();
            })
            .AddTo(this);

        // AI의 생명이 0이 되었을 때 Wid Popup을 띄우는 옵저버
        _battleModel.AIHealthObservable
            .Where(lifePoint => lifePoint == 0)
            .Subscribe(_ =>
            {
                BattleSceneManager.Instance.GameEnd();
                _battleView.ShowWinPopup();
            })
            .AddTo(this);

        // 유저의 데미지 게이지를 갱신하는 옵저버
        _battleModel.UserDamageGageObservable
            .Subscribe(damageGage => _battleView.UpdateDamageGageUI(ActorType.User, damageGage))
            .AddTo(this);

        // AI의 데미지 게이지를 갱신하는 옵저버
        _battleModel.AIDamageGageObservable
            .Subscribe(damageGage => _battleView.UpdateDamageGageUI(ActorType.AI, damageGage))
            .AddTo(this);

        // User의 Ultimate Energy를 갱신하는 옵저버
        _battleModel.UserUltimateEnergyObservable
            .Subscribe(ultimateGage => _battleView.UpdateEnergyBarUI(ActorType.User, ultimateGage));

        // AI의 Ultimate Energy를 갱신하는 옵저버
        _battleModel.AIUltimateEnergyObservable
            .Subscribe(ultimateGage => _battleView.UpdateEnergyBarUI(ActorType.AI, ultimateGage));

        // User의 Ultimate Energy가 가득 찼음을 알리는 옵저버
        _battleModel.UserUltimateEnergyIconObservable.Subscribe(ultimateReady => _battleView.UpdateEnergyIconUI(ActorType.User, ultimateReady));

        // AI의 Ultimate Energy가 가득 찼음을 알리는 옵저버
        _battleModel.AIUltimateEnergyIconObservable.Subscribe(ultimateReady => _battleView.UpdateEnergyIconUI(ActorType.AI, ultimateReady));

        // 게임 종료를 체크하는 옵저버
        Observable.FromAsync(GameEndObservable)
            .Subscribe(_ => _battleView.GoHome())
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

        this.GameCountDownTimerObservable(BattleModel.TimerMax)
            .Subscribe(
                count => _battleModel.SetTimerCountdown(count),
                _ => BattleSceneManager.Instance.GameEnd()).AddTo(this);

        _battleModel.GameCountDownTimerObservable
            .Subscribe(count => _battleView.SetTimerText(count))
            .AddTo(this);


    }

    private async ValueTask GameEndObservable(CancellationToken token)
    {
        await UniTask.WaitUntil(() => BattleSceneManager.Instance.IsEnd, cancellationToken: token);
        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: token);
    }
}