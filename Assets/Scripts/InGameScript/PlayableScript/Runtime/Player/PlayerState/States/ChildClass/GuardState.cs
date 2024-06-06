using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class GuardState : State
    {
        private static readonly int Guard = Animator.StringToHash("Guard");
        private static readonly int GuardStart = Animator.StringToHash("GuardStart");
        private static readonly int GuardEnd = Animator.StringToHash("GuardEnd");
        public GuardState(PlayerStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            StateMachine.StandingVelocity();
            StateMachine.animator.ResetTrigger(GuardEnd);
            StateMachine.AnimSpeed(Data.CharacterType.Frost, 3f);
            StateMachine.animator.SetTrigger(GuardStart);
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Guard).Forget();
            StateMachine.IsGuard = true;
        }


        public override void Exit()
        {
            base.Exit();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Guard);

            StateMachine.animator.SetTrigger(GuardEnd);
            StateMachine.ResetAnimSpeed(0).Forget();

            StateMachine.IsGuard = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (StateMachine.IsDamaged)
                StateMachine.IsDamaged = false;

            if(!StateMachine.guardAction.IsPressed() )
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
