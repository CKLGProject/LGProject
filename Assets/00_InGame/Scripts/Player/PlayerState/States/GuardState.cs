using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class GuardState : State
    {
        GameObject gameObj;
        public GuardState(PlayerStateMachine _stateMachine, GameObject _guardObj) : base(_stateMachine)
        {
            gameObj = _guardObj;
        }

        public override void Enter()
        {
            base.Enter();

            // x,z Velocity를 초기화
            stateMachine.StandingVelocity();
            stateMachine.isGuard = true;
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.isGuard = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(!stateMachine.guardAction.IsPressed())
            {
                stateMachine.guardEffect.SetActive(false);
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}
