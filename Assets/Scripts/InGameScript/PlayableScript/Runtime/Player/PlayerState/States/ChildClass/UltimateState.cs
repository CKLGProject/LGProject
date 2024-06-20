using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LGProject.PlayerState
{
    public class UltimateState : State
    {
        protected const float FocusWeight = 5f;
        protected const float OroginWeight = 1.5f;
        public UltimateState(PlayerStateMachine stateMachine) : base(stateMachine)
        {

        }

        public override void Enter()
        {
            base.Enter();
            
            // 얼티밋을 사용하면 게이지가 닮.
            StateMachine.playable.SetUltimateGage(0);
            // 카메라 확대 
            StateMachine.playable.FocusUltimateUser(FocusWeight);
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