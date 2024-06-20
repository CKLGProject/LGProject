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


        private static readonly int Ultimate = Animator.StringToHash("Ultimate");
       

        public KaneUltimateState(PlayerStateMachine stateMachine, float delay) : base(stateMachine, delay)
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
            //StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePrePoint006).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreCenter).Forget();
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePreRHand).Forget();
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, true);
            _isMove = false;
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
            await UniTask.Delay(TimeSpan.FromSeconds(2f), DelayType.Realtime);
            StateMachine.battleModel.PlayAnimatorControllerTrigger("Hide");
            Time.timeScale = 1f;
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Ultimate);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreRHand);
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.UltimatePreCenter);
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.UltimatePrePoint006).Forget();
            StateMachine.IsUseUltimate = false;

            UltimateAttack();
            StateMachine.playable.FocusUltimateUser(OroginWeight);
            await UniTask.Delay(TimeSpan.FromSeconds(2f), DelayType.Realtime);
            StateMachine.battleModel.ShowCutScene(Data.ActorType.User, false);
        }

        private void UltimateAttack()
        {
            Collider[] checkPlayer = Physics.OverlapBox(StateMachine.transform.position + StateMachine.transform.forward * 2f, new Vector3(4, 1, 1), Quaternion.identity, 1 << 3);
            foreach(var player in checkPlayer)
            {
                Vector3 velocity = StateMachine.transform.forward * 9f + StateMachine.transform.up * 9f;

                if(player.transform != StateMachine.transform)
                {
                    player.GetComponent<Playable>().GetStateMachine.ApplyHitDamaged(velocity, 0, StateMachine);
                }
            }
        }
    }

}