using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class WakeUpState : State
    {
        private float _curTimer = 0;
        private float _delay = 0;
        public WakeUpState(PlayerStateMachine _stateMachine, ref float _delay) : base(_stateMachine)
        {
            this._delay = _delay;   
        }

        public override void Enter()
        {
            base.Enter();
            _curTimer = 0;

            stateMachine.ResetAnimParameters();
            stateMachine.animator.SetTrigger("WakeUp");
            stateMachine.animator.SetFloat("Run", 0);
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.IsDown = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            _curTimer += Time.deltaTime;
            if(_curTimer >= _delay)
            {
                stateMachine.ChangeState(stateMachine.idleState);
                return;
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}