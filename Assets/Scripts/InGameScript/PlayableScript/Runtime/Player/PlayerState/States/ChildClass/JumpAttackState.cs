using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class JumpAttackState : State
    {
        private float _maximumSpeed;
        private static readonly int JumpAttack = Animator.StringToHash("JumpAttack");
        private static readonly int Landing = Animator.StringToHash("Landing");
        public JumpAttackState(PlayerStateMachine stateMachine, float maximumSpeed) : base(stateMachine)
        {
            _maximumSpeed = maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            // Velocity는 초기화시키지 않도록 하자.
            //Debug.Log($"Enter = {StateMachine.JumpInCount}");
            StateMachine.animator.ResetTrigger("Jump1");
            StateMachine.animator.ResetTrigger("Jump2");
            StateMachine.playable.effectManager.Play(EffectManager.EFFECT.JumpAttack, 0.1f).Forget();
            StateMachine.animator.SetTrigger(JumpAttack);
        }

        public override void Exit()
        {
            base.Exit();
            //stateMachine.jumpInCount = 0;
            //Debug.Log($"Exit = {StateMachine.JumpInCount}");
            StateMachine.animator.SetTrigger(Landing);

            StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.JumpAttack);

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            // 점프 공격을 한 상태에서 움직이게 할 것인가?
            // 점프 공격을 한 상태에서 체공 시간을 늘릴 것인가?
            // 1단 점프를 한 후 공격을 한 상태라면 2단 점프가 가능하게 할 것인가?

            if (Mathf.Abs(StateMachine.moveAction.ReadValue<float>()) >= 0.2f)
            {
                // 진행 방향에 적이 있어? 없으면 이동
                if (StateMachine.CheckEnemy() == null && StateMachine.physics.velocity.x <= _maximumSpeed && StateMachine.physics.velocity.x >= -_maximumSpeed)
                    StateMachine.physics.velocity += Vector3.right * (StateMachine.moveAction.ReadValue<float>());
            }
            else
            {
                StateMachine.StandingVelocity();
            }

            // 공격 판정 -> 아직 없음.

            if (StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

        }
    }

}