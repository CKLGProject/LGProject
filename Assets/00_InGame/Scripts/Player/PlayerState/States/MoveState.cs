using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class MoveState : State
    {
        float speed = 0;
        float maximumSpeed = 0;
        public MoveState(PlayerStateMachine _stateMachine, ref float _speed, float _maximumSpeed) : base(_stateMachine)
        {
            speed = _speed;
            maximumSpeed = _maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();

            // ���� ����
            stateMachine.playable.directionX = stateMachine.moveAction.ReadValue<float>() >= 0.1f ? true : false;
            
        
        
        }

        public override void Exit()
        {

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) <= 0.2f)
            {
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return;
            }
            if (stateMachine.attackAction.triggered)
            {
                AttackLogic();
                return;
            }
            if (stateMachine.jumpAction.triggered && stateMachine.jumpInCount < 2)
            {
                stateMachine.ChangeState(stateMachine.playable.jumpState);
                return;
            }

            // ���� ����� �ѹ���!!
            if (stateMachine.guardAction.triggered && !stateMachine.isJumpGuard)
            {
                // ���� �������� ���� ������ ��
                if(!stateMachine.isGrounded )
                {
                    stateMachine.isJumpGuard = true;
                }
                stateMachine.guardEffect.SetActive(true);
                stateMachine.ChangeState(stateMachine.playable.guardState);
                return;
            }

            // ���� ���⿡ ���� �־�?
            if (stateMachine.CheckEnemy())
            {
                stateMachine.StandingVelocity();
                return;
            }
            if (stateMachine.physics.velocity.x <= maximumSpeed && stateMachine.physics.velocity.x >= -maximumSpeed)
            {
                // �ٷ� �տ� ���� ������ ���̻� �̵����� ����(�ִϸ��̼��� ���)
                // �Ӹ��� �ٸ��ʿ��� Ray�� �� ����
                stateMachine.physics.velocity += Vector3.right * (stateMachine.moveAction.ReadValue<float>());
            }

        }

        private void AttackLogic()
        {
            if (!stateMachine.isGrounded)
            {
                stateMachine.ChangeState(stateMachine.playable.jumpAttackState);
                return;
            }
            else
            {
                stateMachine.ChangeState(stateMachine.playable.dashAttackState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}