using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.BossAI
{
    public abstract class State
    {
        protected AiStateMachine _stateMachine;
        public State(AiStateMachine stateMachine) => _stateMachine = stateMachine;


        public virtual void Enter()
        {
            Debug.Log($"Enter State = {this.ToString()}");
        }

        public virtual void Update()
        {

        }

        public virtual void Exit()
        {

        }


    }

}
