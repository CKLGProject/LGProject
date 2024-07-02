using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // 피해를 입었을 때 작동하는 노드
    public class IsDamagedNode : ActionNode
    {
        //public AIAgent Agent;
        protected override void OnStart()
        {


        }

        protected override void OnStop()
        {
            //agent.isHit = false;
            if(AIAgent.Instance.GetStateMachine.IsDamaged)
            {

            }

        }

        protected override State OnUpdate()
        {
            return AIAgent.Instance.GetStateMachine.IsDamaged ? State.Success : State.Failure;
        }
    }

}