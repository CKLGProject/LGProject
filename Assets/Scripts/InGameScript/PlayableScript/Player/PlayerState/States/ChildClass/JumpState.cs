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
        
        public JumpState(PlayerStateMachine _stateMachine, ref float jumpScale, int maximumCount, AnimationCurve animationCurve) : base(_stateMachine)
        {
            //_jumpScale = jumpScale;
            //_maximumCount = maximumCount;
            //_jumpDelay = 0.2f;
            //_animationCurve = animationCurve;
        }

        public override void Enter()
        {
            base.Enter();

            stateMachine.JumpVelocity();

            stateMachine.IsJumpping = true;
            stateMachine.JumpInCount++;
            stateMachine.playable.HandleJumpping();
            //stateMachine.physics.velocity += Vector3.up * _jumpScale;
            stateMachine.animator.SetTrigger("Jump" + stateMachine.JumpInCount.ToString());


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

            if (stateMachine.guardAction.triggered)
            {
                //stateMachine.GuardEffect.SetActive(true);
                stateMachine.ChangeState(stateMachine.guardState);
                return;
            }

            float a = Mathf.Abs(stateMachine.moveAction.ReadValue<float>());
            stateMachine.ChangeState(a >= 0.2f ? stateMachine.moveState : stateMachine.idleState);


        }

        public override void PhysicsUpdate()
        {
            // State

            // 공중에 떠있는 코드.

        }


    }

}