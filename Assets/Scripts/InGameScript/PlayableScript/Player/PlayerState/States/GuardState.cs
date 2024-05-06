using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class GuardState : State
    {
        public GuardState(PlayerStateMachine _stateMachine) : base(_stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();

            // x,z Velocity를 초기화
            
            stateMachine.StandingVelocity();
            stateMachine.animator.SetTrigger("Guard");
            stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Guard);
            stateMachine.isGuard = true;
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Guard);

            stateMachine.animator.SetTrigger("Idle");
            
            stateMachine.isGuard = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.isHit)
                stateMachine.isHit = false;

            if(!stateMachine.guardAction.IsPressed())
            {
                stateMachine.ChangeState(stateMachine.idleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
