using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace LGProject.PlayerState
{


    public class JumpState : State
    {
        //private float _jumpScale = 0;
        //private int _maximumCount = 0;
        //private float _jumpDelay = 0.2f;
        
        private AnimationCurve _animationCurve;

        //Thread callback; 
        
        public JumpState(PlayerStateMachine stateMachine, ref float jumpScale, int maximumCount, AnimationCurve animationCurve) : base(stateMachine)
        {
            //_jumpScale = jumpScale;
            //_maximumCount = maximumCount;
            //_jumpDelay = 0.2f;
            //_animationCurve = animationCurve;
        }

        public override void Enter()
        {
            base.Enter();

            StateMachine.JumpVelocity();

            StateMachine.IsJumpping = true;
            StateMachine.JumpInCount++;
            StateMachine.playable.HandleJumpping();
            //stateMachine.physics.velocity += Vector3.up * _jumpScale;
            StateMachine.animator.SetTrigger("Jump" + StateMachine.JumpInCount);


            // 이걸 n초 뒤에 켜고 싶은데...
            // -> 머리에 닿으면 꺼주고 싶은데...
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (StateMachine.guardAction.triggered)
            {
                //stateMachine.GuardEffect.SetActive(true);
                StateMachine.ChangeState(StateMachine.guardState);
                return;
            }

            float a = Mathf.Abs(StateMachine.moveAction.ReadValue<float>());
            StateMachine.ChangeState(a >= 0.2f ? StateMachine.moveState : StateMachine.idleState);


        }

        public override void PhysicsUpdate()
        {
            // State

            // 공중에 떠있는 코드.

        }


    }

}