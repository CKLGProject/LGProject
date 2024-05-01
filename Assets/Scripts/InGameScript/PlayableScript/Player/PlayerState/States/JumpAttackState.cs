using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class JumpAttackState : State
    {
        float maximumSpeed;
        public JumpAttackState(PlayerStateMachine _stateMachine, float _maximumSpeed) : base(_stateMachine)
        {
            maximumSpeed = _maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            // Velocity는 초기화시키지 않도록 하자.
            Debug.Log($"Enter = {stateMachine.jumpInCount}");
        }

        public override void Exit()
        {
            base.Exit();
            //stateMachine.jumpInCount = 0;
            Debug.Log($"Exit = {stateMachine.jumpInCount}");
            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 점프 공격을 한 상태에서 움직이게 할 것인가?
            // 점프 공격을 한 상태에서 체공 시간을 늘릴 것인가?
            // 1단 점프를 한 후 공격을 한 상태라면 2단 점프가 가능하게 할 것인가?

            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) >= 0.2f)
            {
                // 진행 방향에 적이 있어? 없으면 이동
                if (stateMachine.CheckEnemy() == null && stateMachine.physics.velocity.x <= maximumSpeed && stateMachine.physics.velocity.x >= -maximumSpeed)
                    stateMachine.physics.velocity += Vector3.right * (stateMachine.moveAction.ReadValue<float>());
            }
            else
            {
                stateMachine.StandingVelocity();
            }

            // 공격 판정 -> 아직 없음.

            if (stateMachine.isGrounded)
            {
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