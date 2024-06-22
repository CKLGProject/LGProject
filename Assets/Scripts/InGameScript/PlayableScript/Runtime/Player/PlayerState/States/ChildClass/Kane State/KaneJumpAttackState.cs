using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  LGProject.PlayerState
{
    public class KaneJumpAttackState : JumpAttackState
    {
        float currentTimer = 0f;
        float stopTimer = 0.2f;
        public KaneJumpAttackState(PlayerStateMachine stateMachine, float maximumSpeed, string sfxName) : base(stateMachine, maximumSpeed, sfxName)
        {
            //_maximumSpeed = maximumSpeed;
        }

        public override void Enter()
        {
            base.Enter();
            StateMachine.physics.isKinematic = true;
            //StateMachine.JumpAttackVelocity();
            currentTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            if (Damaged())
                return;
            currentTimer += Time.deltaTime;
            if(currentTimer > stopTimer)
            {
                StateMachine.physics.isKinematic = false;
            }

            if (_damageInCount == false)
                Shoot();

            if (Mathf.Abs(StateMachine.moveAction.ReadValue<Vector2>().x) >= 0.2f)
            {
                // 진행 방향에 적이 있어? 없으면 이동
                if (StateMachine.CheckEnemy() == null && StateMachine.physics.velocity.x <= _maximumSpeed && StateMachine.physics.velocity.x >= -_maximumSpeed)
                    StateMachine.physics.velocity += Vector3.right * StateMachine.moveAction.ReadValue<Vector2>().x;
            }
            else
            {
                StateMachine.StandingVelocity();
            }

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
        private void Shoot()
        {
            _damageInCount = true;
            Debug.Log("JumpAtk");
            StateMachine.ShootProjectile();
        }
    }

}