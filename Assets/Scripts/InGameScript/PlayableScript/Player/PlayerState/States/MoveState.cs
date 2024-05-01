using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
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

            // 방향 판정
            stateMachine.playable.directionX = stateMachine.moveAction.ReadValue<float>() >= 0.1f ? true : false;
        
        
        }

        public override void Exit()
        {

        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(stateMachine.isGrounded)
            {
                stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Run);
            }
            else
            {
                stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Run);
            }

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

            // 점프 가드는 한번만!!
            if (stateMachine.guardAction.triggered && !stateMachine.isJumpGuard)
            {
                // 땅에 접촉하지 않은 상태일 때
                if(!stateMachine.isGrounded )
                {
                    stateMachine.isJumpGuard = true;
                }
                stateMachine.guardEffect.SetActive(true);
                stateMachine.ChangeState(stateMachine.playable.guardState);
                return;
            }

            // 진행 방향에 적이 있어?
            if (stateMachine.CheckEnemy())
            {
                stateMachine.StandingVelocity();
                return;
            }
            if (stateMachine.physics.velocity.x <= maximumSpeed && stateMachine.physics.velocity.x >= -maximumSpeed)
            {
                // 바로 앞에 적이 있으면 더이상 이동하지 않음(애니메이션은 재생)
                // 머리와 다리쪽에서 Ray를 쏠 예정
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