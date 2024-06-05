using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class GuardState : State
    {
        private static readonly int Guard = Animator.StringToHash("Guard");

        public GuardState(PlayerStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            // x,z Velocity를 초기화

            StateMachine.StandingVelocity();
            StateMachine.animator.ResetTrigger("GuardEnd");
            StateMachine.animator.SetTrigger("GuardStart");
            //StateMachine.animator.SetBool(Guard, true);
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Guard).Forget();
            StateMachine.IsGuard = true;
        }

        public override void Exit()
        {
            base.Exit();
            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Guard);

            //StateMachine.animator.SetBool(Guard, false);
            StateMachine.animator.SetTrigger("GuardEnd");

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
