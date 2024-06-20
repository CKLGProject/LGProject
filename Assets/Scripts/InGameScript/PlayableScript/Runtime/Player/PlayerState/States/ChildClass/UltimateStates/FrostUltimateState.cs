using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

namespace LGProject.PlayerState
{
    public class FrostUltimateState : UltimateState
    {
        // 자기 강화 궁극기
        // 지속시간 5초


        private static readonly int Ultimate = Animator.StringToHash("Ultimate");
        

        public FrostUltimateState(PlayerStateMachine stateMachine, float delay) : base(stateMachine, delay)
        {

        }
        public override void Enter()
        {
            base.Enter();
            // 시간 설정
            //StateMachine.SetUltimateState();
            StateMachine.ResetVelocity();
            StateMachine.animator.SetTrigger(Ultimate);

            StateMachine.IsUseUltimate = true;
            StateMachine.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Ultimate).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreCenter).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreRHand).Forget();
            _isMove = false;
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, true);
            UsingUltimateSkill().Forget();
            WaitStart().Forget();
            Time.timeScale = 0.1f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if(_isMove)
            {
                StateMachine.IsUltimate = true;
                StateMachine.ChangeState(StateMachine.idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }

        protected async UniTaskVoid WaitStart()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.Realtime);
            StateMachine.playable.SwitchingWeapon(true);
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, false);
            //StateMachine.battleModel.PlayAnimatorControllerTrigger("Hide");
            Time.timeScale = 1f;
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimateHit).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimateDash).Forget();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Ultimate);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreRHand);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreCenter);
            StateMachine.IsUseUltimate = false;
            // 내리찍을 때 주변 적에게 피해를 입히고 넉백시킴.
            StateMachine.playable.FocusUltimateUser(OriginWeight);
            ShockWake();
            await UniTask.Delay(TimeSpan.FromSeconds(2f), DelayType.Realtime);
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, false);
        }

        // 프로스트가 내리찍을 때 작동하는 기능 
        private void ShockWake()
        {
            Collider[] checkPlayer = Physics.OverlapSphere(StateMachine.transform.position, 3, 1 << 3);

            foreach (var player in checkPlayer)
            {
                Vector3 velocity = (player.transform.position - StateMachine.transform.position + Vector3.up).normalized;
                velocity *= 10f;
                if (player.transform != StateMachine.transform)
                {
                    //Debug.Log($"{player.transform.name}");
                    player.GetComponent<Playable>().GetStateMachine.ApplyHitDamaged(velocity, 0, StateMachine);
                }
            }
        }

    }
}