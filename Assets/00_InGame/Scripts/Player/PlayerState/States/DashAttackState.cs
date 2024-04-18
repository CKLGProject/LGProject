using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PlayerState
{
    // 상속은 다음 기회에 
    public class DashAttackState : State
    {
        float curTimer;
        float aniDelay = 0;
        public DashAttackState(PlayerStateMachine _stateMachine, ref float _aniDelay) : base(_stateMachine)
        {
            aniDelay = _aniDelay;
        }

        public override void Enter()
        {
            base.Enter();
            curTimer = 0;
            // velocity 초기화 X
            // 그런데 브레이크는 걸면 좋을 듯? 대충 Drag값 조절해서 끼이익 하는 느낌을 줘보자.

        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            curTimer += Time.deltaTime; 
            if(curTimer >= aniDelay)
            {
                stateMachine.ChangeState(stateMachine.playable.idleState);
                return ;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
        }

        public override void Exit()
        {
            base.Exit();
        }

    }

}