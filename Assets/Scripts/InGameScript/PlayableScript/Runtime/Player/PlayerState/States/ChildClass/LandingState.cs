using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class LandingState : State
    {
        private float _currentTimer;
        private static readonly int Landing = Animator.StringToHash("Landing");

        public LandingState(PlayerStateMachine stateMachine) : base (stateMachine)
        {
            
        }

        public override void Enter()
        {
            base.Enter();

            StateMachine.physics.isKinematic = false;
            StateMachine.StandingVelocity();
            StateMachine.animator.SetTrigger(Landing);
            _currentTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();

            if (Damaged())
                return;
            _currentTimer += Time.deltaTime;
            if (_currentTimer > 0.25f)
            {
                if (Mathf.Abs(StateMachine.moveAction.ReadValue<Vector2>().x) > 0.2f)
                    StateMachine.ChangeState(StateMachine.moveState);
                else
                    StateMachine.ChangeState(StateMachine.idleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}