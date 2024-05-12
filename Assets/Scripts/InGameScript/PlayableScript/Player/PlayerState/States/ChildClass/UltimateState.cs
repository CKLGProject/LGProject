using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class UltimateState : State
    {
        // 기술을 사용하면 자신을 제외한 모든 오브젝트는 정지된다!
        // 배경은 움직임
        // 공격을 진행 시 

        public UltimateState(PlayerStateMachine _stateMachine) : base(_stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}