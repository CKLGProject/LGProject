using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class MoveState : State
    {
        private float _speed = 0;
        private float _maximumSpeed = 0;
        private bool _isPlayingMoveEffect = false;
        public MoveState(PlayerStateMachine _stateMachine, ref float _speed, float _maximumSpeed) : base(_stateMachine)
        {
            this._speed = _speed;
            this._maximumSpeed = _maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();

            // 방향 판정
            stateMachine.playable.directionX = stateMachine.moveAction.ReadValue<float>() >= 0.1f ? true : false;
        
        
        }

        public override void Exit()
        {
            stateMachine.animator.SetFloat("Run", 0);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            stateMachine.animator.SetFloat("Run", Mathf.Abs(stateMachine.moveAction.ReadValue<float>()));
            if (stateMachine.IsGrounded && !_isPlayingMoveEffect)
            {
                stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Run).Forget();
                _isPlayingMoveEffect = true;
            }
            else if(!stateMachine.IsGrounded && _isPlayingMoveEffect)
            {
                _isPlayingMoveEffect = false;
                stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Run);
            }

            if (Mathf.Abs(stateMachine.moveAction.ReadValue<float>()) <= 0.2f)
            {
                stateMachine.ChangeState(stateMachine.idleState);
                return;
            }
            if (stateMachine.attackAction.triggered)
            {
                AttackLogic();
                return;
            }
            if (stateMachine.jumpAction.triggered && stateMachine.JumpInCount < 2)
            {
                stateMachine.ChangeState(stateMachine.jumpState);
                return;
            }

            // 점프 가드는 한번만!!
            if (stateMachine.guardAction.triggered && !stateMachine.IsJumpGuard)
            {
                // 땅에 접촉하지 않은 상태일 때
                if(!stateMachine.IsGrounded )
                {
                    stateMachine.IsJumpGuard = true;
                }
                stateMachine.ChangeState(stateMachine.guardState);
                return;
            }

            // 진행 방향에 적이 있어?
            if (stateMachine.CheckEnemy())
            {
                stateMachine.StandingVelocity();
                return;
            }
            if (stateMachine.physics.velocity.x <= _maximumSpeed && stateMachine.physics.velocity.x >= -_maximumSpeed)
            {
                // 바로 앞에 적이 있으면 더이상 이동하지 않음(애니메이션은 재생)
                // 머리와 다리쪽에서 Ray를 쏠 예정
                stateMachine.physics.velocity += Vector3.right * (stateMachine.moveAction.ReadValue<float>());

                Vector3 left = new Vector3((stateMachine.transform.position + Vector3.right).x + 2f, stateMachine.transform.position.y, stateMachine.transform.position.z);
                Vector3 right = new Vector3((stateMachine.transform.position + Vector3.left).x - 2f, stateMachine.transform.position.y, stateMachine.transform.position.z);

                stateMachine.transform.LookAt(stateMachine.moveAction.ReadValue<float>() < 0 ? right : left);
            }

        }

        private void AttackLogic()
        {
            if (stateMachine.UltimateGage >= 100)
            {
                stateMachine.ChangeState(stateMachine.ultimateState);
                return;
            }
            else if (!stateMachine.IsGrounded)
            {
                stateMachine.ChangeState(stateMachine.jumpAttackState);
                return;
            }
            else if(stateMachine.IsGrounded && Mathf.Abs(stateMachine.physics.velocity.x) > 0.2f)
            {
                stateMachine.ChangeState(stateMachine.dashAttackState);
                return;
            }
            else
            {
                stateMachine.ChangeState(stateMachine.attackState);
            } 
                
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}