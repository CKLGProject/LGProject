using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{
    public class WakeUpState : State
    {
        private float _currentTimer;
        private float _delay;
        private static readonly int WakeUp = Animator.StringToHash("WakeUp");
        private static readonly int Run = Animator.StringToHash("Run");

        public WakeUpState(PlayerStateMachine stateMachine, ref float delay) : base(stateMachine)
        {
            _delay = delay;   
        }

        public override void Enter()
        {
            base.Enter();
            _currentTimer = 0;

            StateMachine.ResetAnimationParameters();
            StateMachine.animator.SetTrigger(WakeUp);
            StateMachine.animator.SetFloat(Run, 0);
        }

        public override void Exit()
        {
            base.Exit();
            StateMachine.IsDown = false;
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            _currentTimer += Time.deltaTime;
            if(_currentTimer >= _delay)
            {
                StateMachine.ChangeState(StateMachine.idleState);
                return;
            }

        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}