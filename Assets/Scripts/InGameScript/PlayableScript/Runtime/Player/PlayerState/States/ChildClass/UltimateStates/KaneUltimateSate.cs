using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace LGProject.PlayerState
{
    public class KaneUltimateState : UltimateState
    {
        // 슈우우우우웅 탁! 느낌이라
        // 흠 어케하면 좋을까...

        private bool _isMove;

        private static readonly int Ultimate = Animator.StringToHash("Ultimate");


        public KaneUltimateState(PlayerStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            StateMachine.ResetVelocity();
            StateMachine.animator.SetTrigger(Ultimate);

            StateMachine.IsUseUltimate = true;
            StateMachine.animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Ultimate).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreCenter).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreRHand).Forget();
            _isMove = false;
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

            if (_isMove)
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
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
            //StateMachine.playable.SwitchingWeapon(true);
            //await UniTask.Delay(TimeSpan.FromSeconds(0.075f));
            _isMove = true;
            Time.timeScale = 1f;
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimateHit).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimateDash).Forget();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Ultimate);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreRHand);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreCenter);
            StateMachine.IsUseUltimate = false;
            // 내리찍을 때 주변 적에게 피해를 입히고 넉백시킴.
            UltimateAttack();
        }

        private void UltimateAttack()
        {
            // 전방을 향해 힘찬 함성 5초간 발싸!
            // 으앙ㅇ아아ㅏㅇ아ㅏ아ㅏㅏ아아아아아ㅏ아ㅏㅏㅇㅇㅇ아ㅏ아아아ㅏ아ㅏㅏ아ㅏㅏ아ㅏ아앙아아ㅏㅏ
            Collider[] checkPlayer = Physics.OverlapBox(StateMachine.transform.position + StateMachine.transform.forward * 3f, new Vector3(6, 1, 1), Quaternion.identity, 1 << 3);
            foreach(var player in checkPlayer)
            {
                Vector3 velocity = StateMachine.transform.forward * 4.5f + StateMachine.transform.up * 4.5f;

                if(player.transform != StateMachine.transform)
                {
                    player.GetComponent<Playable>().GetStateMachine.ApplyHitDamaged(velocity, 0, StateMachine);
                }
            }
        }
    }

}