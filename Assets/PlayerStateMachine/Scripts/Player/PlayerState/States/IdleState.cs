using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState
{
    public class IdleState : PlayerState.State
    {

        public IdleState(PlayerState.PlayerStateMachine _stateMachine) : base (_stateMachine)
        {

        }

        public override void Enter()
        {
            // ���� �� ���� ������ �ʱ�ȭ.
            base.Enter();
            stateMachine.StandingVelocity();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // Ű �Է��� ��� ������ ���°� �����.
            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) > 0.2f)
            {
                stateMachine.ChangeState(stateMachine.playable.moveState);
                return;
            }
            //Debug.Log($"Attack{stateMachine.attackAction.triggered}");
            //bool jump = jumpAction.ReadValue<bool>();
            if (stateMachine.attackAction.triggered)
            {
                AttackLogic();
                return;
            }
            if (stateMachine.jumpAction.triggered && stateMachine.jumpInCount < 2)
            {
                //Debug.Log("idleJump");
                stateMachine.ChangeState(stateMachine.playable.jumpState);
                return;
            }

            // ���� �߿��� �� ���� �����ϰ� �Ѵ�.
            if (stateMachine.guardAction.triggered && !stateMachine.isJumpGuard)
            {
                // ���� �������� ���� ������ ��
                if (!stateMachine.isGrounded)
                {
                    stateMachine.isJumpGuard = true;
                }
                stateMachine.guardEffect.SetActive(true);
                stateMachine.ChangeState(stateMachine.playable.guardState);
                return;
            }

            //bool Down = guardAction.ReadValue<bool>();

            if (stateMachine.downAction.triggered)
            {
                Debug.Log($"Down = {true}");
            }
        }

        void AttackLogic()
        {
            // ���� �پ������鼭 ������ �����ϸ�?
            if (stateMachine.isGrounded)
            {
                stateMachine.ChangeState(stateMachine.playable.attackState);
                return;
            }
            // ���߿��� �����ϸ�?
            if(!stateMachine.isGrounded)
            {
                stateMachine.ChangeState(stateMachine.playable.jumpAttackState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            // ���� ������ üũ�Ͽ� jump Count�� 0���� ����.
            base.PhysicsUpdate();

        }

        public override void Exit()
        {
            // ���� �� ���ʿ��� �������� ����.

        }

    }

}