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

            // 점프 후 이동과 추가 점프를 체크해야함.
            if (StateMachine.JumpInCount < 2 && StateMachine.jumpAction.triggered)
            {
                // 점프 다시 시작. 점프는 두번만 인식하기
                StateMachine.ChangeState(StateMachine.jumpState);
            }

            if (StateMachine.guardAction.triggered)
            {
                //stateMachine.GuardEffect.SetActive(true);
                StateMachine.ChangeState(StateMachine.guardState);
                return;
            }
            if(StateMachine.IsGrounded)
            {
                StateMachine.ChangeState(StateMachine.landingState);
            }
            Movement(3f);
            //if (StateMachine.)
        }

        public override void PhysicsUpdate()
        {
            // State

            // 공중에 떠있는 코드.

        }


    }

}