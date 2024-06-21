using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class HitState : State
    {
        private float _stunTimer;

        private float _currentTimer; 
        // 피격 당했을 떄 날아가나?
        public HitState(PlayerStateMachine stateMachine, ref float stunTimer) : base(stateMachine)
        {
            _stunTimer = stunTimer;
            //_stunTimer = stunTimer;
        }
        public override void Enter()
        {
            base.Enter();
            // x,z Velocity를 초기화

            StateMachine.AttackCount = 0;
            _currentTimer = 0;

            // 피격 당했을 때 퍽 FX 출력
        }

        public override void Exit()
        {
            base.Exit();
            _currentTimer = 0;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            StateMachine.IsDamaged = false;
            if (!StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.knockbackState);
                return;
            }
            else
            {
                _currentTimer += Time.deltaTime;
                // 공중에서 피격당할 시 Idle state가 아닌 Down State로 변경
                // Down State의 경우 땅에 닿을 때(is Grounded = true) 까지
                // Down State를 유지해야 하며, 
                if(_currentTimer > _stunTimer)
                {
                    StateMachine.ChangeState(StateMachine.idleState);
                }
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }
    }
}
