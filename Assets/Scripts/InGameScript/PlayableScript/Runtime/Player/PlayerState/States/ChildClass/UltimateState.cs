using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGProject.PlayerState
{
    public class UltimateState : State
    {
        public UltimateState(PlayerStateMachine _stateMachine) : base(_stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            
            // 얼티밋을 사용하면 게이지가 닮.
            stateMachine.playable.SetUltimateGage(0);
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