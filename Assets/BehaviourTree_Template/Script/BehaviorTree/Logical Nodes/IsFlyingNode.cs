using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class IsFlyingNode : ActionNode
    {
        public AIAgent Agent;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        // 공중에 있는지 체크하는 노드
        // 어디서 부터 들어왔냐에 따라 다음 진행할 노드가 달라짐.
        protected override State OnUpdate()
        {
            if(Agent.GetStateMachine.isGrounded)
            {
                return State.Success;
            }

            return State.Failure;
        }
    }

}