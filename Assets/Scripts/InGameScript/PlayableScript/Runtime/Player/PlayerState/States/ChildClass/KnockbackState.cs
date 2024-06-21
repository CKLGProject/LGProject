using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class KnockbackState : State
    {
        public KnockbackState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            // 날아가는 중임을 체크
            //StateMachine.IsKnockback = true;
            //StateMachine.animator.SetTrigger("Knockback");
            StateMachine.physics.isKinematic = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            if(StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.downState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}