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
        }
        public override void Enter()
        {
            base.Enter();
            // x,z Velocity를 초기화
            //Vector3 v = stateMachine.physics.velocity;
            //Vector3.zero;
            //stateMachine.physics.velocity = stateMachine;

            StateMachine.IsDamaged = false;     // 맞았어! 맞았다고! 그만때려!
            StateMachine.AttackCount = 0;
            //Debug.Log($"{stateMachine.transform.ToString()}who's hit? : {stateMachine.hitPlayer}");
            _currentTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
            _currentTimer = 0;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(!StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.knockbackState);
                return;
                //curTimer += Time.deltaTime;
                //if (curTimer >= stunedTimer)
                //{
                //}
            }
            else
            {
                // 공중에서 피격당할 시 Idle state가 아닌 Down State로 변경
                // Down State의 경우 땅에 닿을 때(is Grounded = true) 까지
                // Down State를 유지해야 하며, 
                StateMachine.ChangeState(StateMachine.idleState);
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }
    }
}
