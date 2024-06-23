using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
namespace LGProject.PlayerState
{
    public class UltimateState : State
    {
        protected const float FocusWeight = 5f;
        protected const float OriginWeight = 1.5f;
        protected float UltimateDelay = 0f;
        protected bool _isMove;
        protected string UltimateReady = string.Empty;
        protected string UltimateSFXName = string.Empty;
        public UltimateState(PlayerStateMachine stateMachine, float delay) : base(stateMachine)
        {
            UltimateDelay = delay;
            UltimateReady = "UltimateReady";
        }

        public override void Enter()
        {
            base.Enter();
            
            // 얼티밋을 사용하면 게이지가 닮.
            StateMachine.playable.SetUltimateGage(0);
            // 카메라 확대 
            _isMove = false;
            StateMachine.playable.FocusUltimateUser(FocusWeight);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        protected async UniTaskVoid UsingUltimateSkill()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(UltimateDelay), DelayType.Realtime);
            _isMove = true;
        }


    }

}