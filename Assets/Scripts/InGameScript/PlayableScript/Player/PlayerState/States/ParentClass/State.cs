using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LGProject.PlayerState
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
            //Debug.Log($"Enter State = {this.ToString()}");
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void LogicUpdate()
        {
            if(stateMachine.IsDamaged && !stateMachine.IsGuard)
            {
                // 공격을 받았을 때, hitState로 변경해 줌.
                
                stateMachine.ChangeState(stateMachine.hitState);
            }
        }

        public virtual  void Exit()
        {

        }

    }

}