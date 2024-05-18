using Data;
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

        // 유저 프로필 이미지를 설정합니다.
        ECharacterType character = CurrentGameManager.CurrentCharacter();
        _battleView.SetUserProfileImage(BattleView.Target.User, character);
        
        // 유저 정령 처리
        EPatType pat = CurrentGameManager.CurrentPat(); 
        _battleView.SetPatNameText(BattleView.Target.User, pat);

        
        // 로딩 텍스트 렌더링 옵저버
        _battleView.UpdateLoadingTextObservable()
            .Subscribe(_ => _battleView.UpdateLoadingText()).AddTo(this);
    }
}