using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace PlayerState
{
    //public class AAAA
    //{
    //    AAAA
    //}

    public class JumpState : State
    {
        float jumpScale = 0;
        int maximumCount = 0;
        float jumpDelay = 0.2f;
        //Thread callback; 
        
        public JumpState(PlayerStateMachine _stateMachine, ref float _jumpScale, int _maximumCount) : base(_stateMachine)
        {
            jumpScale = _jumpScale;
            maximumCount = _maximumCount;
            jumpDelay = 0.2f;
            //callback = new Thread(new ThreadStart(Run));
        }

        //void Run()
        //{

        //}

        public override void Enter()
        {
            base.Enter();

            stateMachine.JumpVelocity();

            stateMachine.jumpInCount++;
            stateMachine.physics.velocity += Vector3.up * jumpScale;


        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (stateMachine.guardAction.triggered)
            {
                stateMachine.guardEffect.SetActive(true);
                stateMachine.ChangeState(stateMachine.playable.guardState);
                return;
            }

            float a = Mathf.Abs(stateMachine.moveAction.ReadValue<float>());
            stateMachine.ChangeState(a >= 0.2f ? stateMachine.playable.moveState : stateMachine.playable.idleState);


        }

        public override void PhysicsUpdate()
        {
            // State

            // 공중에 떠있는 코드.

        }

    }

}