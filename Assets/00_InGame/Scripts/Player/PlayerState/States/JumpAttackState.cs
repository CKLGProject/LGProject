using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerState
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
            // Velocity�� �ʱ�ȭ��Ű�� �ʵ��� ����.
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
            // ���� ������ �� ���¿��� �����̰� �� ���ΰ�?
            // ���� ������ �� ���¿��� ü�� �ð��� �ø� ���ΰ�?
            // 1�� ������ �� �� ������ �� ���¶�� 2�� ������ �����ϰ� �� ���ΰ�?

            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) >= 0.2f)
            {
                // ���� ���⿡ ���� �־�?
                if (!stateMachine.CheckEnemy() && stateMachine.physics.velocity.x <= maximumSpeed && stateMachine.physics.velocity.x >= -maximumSpeed)
                    stateMachine.physics.velocity += Vector3.right * (stateMachine.moveAction.ReadValue<float>());
            }
            else
            {
                stateMachine.StandingVelocity();
            }

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