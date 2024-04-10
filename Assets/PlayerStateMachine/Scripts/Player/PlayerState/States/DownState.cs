using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    // ���� �ִ� State
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
                // ���� ����� �� �����ִ� State
                curTimer += Time.deltaTime;
                if (curTimer >= delay)
                {
                    // ������ WakeUp State
                    // Wake UP �߿��� ������ �޾Ƶ� ������.
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