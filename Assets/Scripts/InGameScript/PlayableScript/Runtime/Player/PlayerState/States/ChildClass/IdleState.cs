using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class IdleState : State
    {
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Run = Animator.StringToHash("Run");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Landing = Animator.StringToHash("Landing");

        public IdleState(PlayerStateMachine stateMachine) : base (stateMachine)
        {

        }

        public override void Enter()
        {
            // 입장 시 내부 정보를 초기화.
            base.Enter();
            StateMachine.StandingVelocity();
            StateMachine.animator.SetTrigger(Idle);
            StateMachine.animator.SetFloat(Run, 0);
            StateMachine.animator.SetInteger(Attack, 0);
            StateMachine.animator.ResetTrigger(Landing);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 키 입력을 대기 받으면 상태가 변경됨.
            if (Mathf.Abs(StateMachine.moveAction.ReadValue<float>()) > 0.2f )
            {
                StateMachine.ChangeState(StateMachine.moveState);
                return;
            }

            if (StateMachine.attackAction.triggered)
            {
                AttackLogic();
                return;
            }

            if (StateMachine.jumpAction.triggered && StateMachine.JumpInCount < 2)
            {
                //Debug.Log("idleJump");
                StateMachine.ChangeState(StateMachine.jumpState);
                return;
            }

            // 점프 중에는 한 번만 가능하게 한다.
            if (StateMachine.guardAction.triggered && StateMachine.GuardGage > 0)
            {
                StateMachine.ChangeState(StateMachine.guardState);
                return;
            }

            //bool Down = guardAction.ReadValue<bool>();

            if (StateMachine.downAction.triggered)
            {
                Debug.Log($"Down = {true}");
            }
        }

        private void AttackLogic()
        {
            // 땅에 붙어있으면서 공격을 진행하면?
            if(StateMachine.playable.UltimateGage >= 100)
            {
                StateMachine.ChangeState(StateMachine.ultimateState);
                return;
            }

            if (StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.attackState);
                return;
            }
            
            // 공중에서 공격하면?
            if(!StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.jumpAttackState);
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