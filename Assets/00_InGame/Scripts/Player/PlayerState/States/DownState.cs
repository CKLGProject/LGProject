using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    // 누워 있는 State
    public class DownState : State
    {
        float curTimer = 0;
        float delay = 0;
        public DownState(PlayerStateMachine _stateMachine, float _delay) : base(_stateMachine)
        {
            curTimer = 0;
            delay = _delay;
        }

        public override void Enter()
        {
            base.Enter();
            curTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.isGrounded)
            {
                // 땅에 닿았을 때 누워있는 State
                curTimer += Time.deltaTime;
                if (curTimer >= delay)
                {
                    // 원래는 WakeUp State
                    // Wake UP 중에는 공격을 받아도 무적임.
                    stateMachine.ChangeState(stateMachine.playable.idleState);
                    return;
                }
            }
            else
            {
                curTimer = 0;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}