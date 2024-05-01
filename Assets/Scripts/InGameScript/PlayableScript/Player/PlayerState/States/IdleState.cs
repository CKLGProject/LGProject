using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class IdleState : State
    {

        public IdleState(PlayerStateMachine _stateMachine) : base (_stateMachine)
        {

        }

        public override void Enter()
        {
            // 입장 시 내부 정보를 초기화.
            base.Enter();
            stateMachine.StandingVelocity();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 키 입력을 대기 받으면 상태가 변경됨.
            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) > 0.2f)
            {
                stateMachine.ChangeState(stateMachine.playable.moveState);
                return;
            }

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

            // 점프 중에는 한 번만 가능하게 한다.
            if (stateMachine.guardAction.triggered && !stateMachine.isJumpGuard)
            {
                // 땅에 접촉하지 않은 상태일 때
                if (!stateMachine.isGrounded)
                {
                    stateMachine.isJumpGuard = true;
                }
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
            // 땅에 붙어있으면서 공격을 진행하면?
            if (stateMachine.isGrounded)
            {
                stateMachine.ChangeState(stateMachine.playable.attackState);
                return;
            }
            // 공중에서 공격하면?
            if(!stateMachine.isGrounded)
            {
                stateMachine.ChangeState(stateMachine.playable.jumpAttackState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            // 땅에 닿은지 체크하여 jump Count를 0으로 변경.
            base.PhysicsUpdate();

        }

        public override void Exit()
        {
            // 나갈 떄 불필요한 정보들을 정리.

        }

    }

}