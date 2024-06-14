using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class FlightState : State
    {
        private static readonly int Landing = Animator.StringToHash("Landing");
        private float dontInputKeyTimer = 0.0f;
        private float currentTimer = 0;
        public FlightState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            //StateMachine.IsGrounded = false;
            //StateMachine.JumpVelocity();
            StateMachine.animator.ResetTrigger(Landing);
            StateMachine.IsJumpping = true;
            StateMachine.JumpInCount++;
            //StateMachine.playable.HandleJumpping();
            //stateMachine.physics.velocity += Vector3.up * _jumpScale;
            StateMachine.animator.SetTrigger("Jump" + StateMachine.JumpInCount);
            FileManager.Instance.SetInGameData(DATA_TYPE.Jump);

        }

        public override void Exit()
        {
            base.Exit();
            currentTimer = 0;
        }

        public override void LogicUpdate()
        {
            if (Damaged())
                return;
            // 점프 후 이동과 추가 점프를 체크해야함.
            if (StateMachine.JumpInCount < 2 && StateMachine.jumpAction.triggered)
            {
                // 점프 다시 시작. 점프는 두번만 인식하기
                StateMachine.ChangeState(StateMachine.jumpState);
            }

            currentTimer += Time.deltaTime;
            if (currentTimer > dontInputKeyTimer)
            {
                // 가드 게이지가 1이상일 경우 발동
                if (StateMachine.guardAction.triggered && StateMachine.GuardGage > 0 && !StateMachine.IsJumpGuard)
                {
                    //stateMachine.GuardEffect.SetActive(true);
                    StateMachine.IsJumpGuard = true;
                    StateMachine.ChangeState(StateMachine.guardState);
                    return;
                }

                if (StateMachine.attackAction.triggered)
                {
                    StateMachine.ChangeState(StateMachine.jumpAttackState);
                }
            }
            if (StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.landingState);
            }
            Movement(3f);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}