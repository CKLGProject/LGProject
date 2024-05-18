using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using USingleton;
using Random = UnityEngine.Random;

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

        // 유저 이름을 설정합니다.
        string userName = CurrentGameManager.GetNickname();
        _battleView.SetUserNameText(BattleView.Target.User, userName);

        //TODO:유저 프로필 이미지를 설정해야합니다.
        // _battleView.SetUserProfileImage();
        
        // 유저 정령 처리
        int patDataMaxCount = CurrentGameManager.GetPatDataCount();
        int userPatRandom = Random.Range(0, patDataMaxCount);
        _battleView.SetPatNameText(BattleView.Target.User, "Fuxk");



        // 로딩 텍스트 렌더링 옵저버
        _battleView.UpdateLoadingTextObservable()
            .Subscribe(_ => _battleView.UpdateLoadingText()).AddTo(this);
    }
}