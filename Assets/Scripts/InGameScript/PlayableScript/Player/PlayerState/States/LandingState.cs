using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class LandingState : State
    {
        float curTimer = 0;

        public LandingState(PlayerStateMachine _stateMachine) : base (_stateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();
            stateMachine.StandingVelocity();
            stateMachine.animator.SetTrigger("Landing");
            curTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            curTimer += Time.deltaTime;
            if (curTimer > stateMachine.GetAnimPlayTime("JumpEnd"))
            {
                if(Mathf.Abs(stateMachine.moveAction.ReadValue<float>())> 0.2f)
                {
                    stateMachine.ChangeState(stateMachine.moveState);
                }
                else
                    stateMachine.ChangeState(stateMachine.idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}