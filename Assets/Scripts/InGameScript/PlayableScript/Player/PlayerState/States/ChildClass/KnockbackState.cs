using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class KnockbackState : State
    {
        public KnockbackState(PlayerStateMachine _stateMachine) : base(_stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // 날아가는 중임을 체크
            stateMachine.IsKnockback = true;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(stateMachine.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.downState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}