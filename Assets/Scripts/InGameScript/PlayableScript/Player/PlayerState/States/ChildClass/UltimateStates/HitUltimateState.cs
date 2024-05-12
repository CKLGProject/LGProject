using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class HitUltimateState : UltimateState
    {
        private float _curTime = 0;
        private float _delay = 0;

        public HitUltimateState(PlayerStateMachine _stateMachine, float _delayTime) : base(_stateMachine)
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
            // 로직의 경우 한 번 누르면 작동함.

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}