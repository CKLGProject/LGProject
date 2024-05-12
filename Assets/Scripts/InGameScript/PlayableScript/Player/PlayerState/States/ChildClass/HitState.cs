using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class HitState : State
    {
        float stunedTimer;
        float curTimer; 
        // 피격 당했을 떄 날아가나?
        public HitState(PlayerStateMachine _stateMachine, ref float _stunedTimer) : base(_stateMachine)
        {
            stunedTimer = _stunedTimer;
        }
        public override void Enter()
        {
            base.Enter();
            // x,z Velocity를 초기화
            //Vector3 v = stateMachine.physics.velocity;
            //Vector3.zero;
            //stateMachine.physics.velocity = stateMachine;

            stateMachine.IsDamaged = false;     // 맞았어! 맞았다고! 그만때려!
            stateMachine.AttackCount = 0;
            //Debug.Log($"{stateMachine.transform.ToString()}who's hit? : {stateMachine.hitPlayer}");
            curTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
            curTimer = 0;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if(stateMachine.IsGrounded)
            {
                curTimer += Time.deltaTime;
                if (curTimer >= stunedTimer)
                {
                    stateMachine.ChangeState(stateMachine.downState);
                    return;
                }
            }
            else
            {
                // 공중에서 피격당할 시 Idle state가 아닌 Down State로 변경
                // Down State의 경우 땅에 닿을 때(is Grounded = true) 까지
                // Down State를 유지해야 하며, 
                stateMachine.ChangeState(stateMachine.downState);
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }
    }
}
