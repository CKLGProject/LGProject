using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class FrostUltimateState : UltimateState
    {
        private float _curTimer;
        private float _delay;
        public FrostUltimateState(PlayerStateMachine _stateMachine, float _delayTime) : base(_stateMachine)
        {
            _delay = _delayTime;
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