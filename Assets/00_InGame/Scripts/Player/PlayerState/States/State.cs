using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerState
{
    public abstract class State
    {
        protected PlayerStateMachine stateMachine;

        RaycastHit hit;

        // 키 입력에 다른 Action을 출력

        public State(PlayerState.PlayerStateMachine _stateMachine) 
        {
            stateMachine = _stateMachine;

        }
        public virtual void Enter()
        {
            //Debug.Log($"{stateMachine.playable.ToString()} / Enter State = {this.ToString()}");
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void LogicUpdate()
        {
            if(stateMachine.isHit)
            {
                // 공격을 받았을 때, hitState로 변경해 줌.
                stateMachine.ChangeState(stateMachine.playable.hitState);
            }
            Debug.DrawLine(stateMachine.transform.position + Vector3.down, stateMachine.transform.position + Vector3.down * 1.1f, Color.red);

            //if (Physics.Raycast(stateMachine.transform.position + Vector3.down, Vector3.down, out hit, maxDistance: 0.5f))
            //{
            //    stateMachine.isGrounded = true;
            //    stateMachine.jumpInCount = 0;
            //}
            //else
            //{
            //    stateMachine.isGrounded = false;
            //}
        }

        public virtual  void Exit()
        {

        }

    }

}