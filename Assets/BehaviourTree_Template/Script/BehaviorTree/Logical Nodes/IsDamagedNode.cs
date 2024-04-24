using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    // 피해를 입었을 때 작동하는 노드
    public class IsDamagedNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {
            //agent.isHit = false;
        }

        protected override State OnUpdate()
        {
            return agent.GetStateMachine.isHit ? State.Success : State.Failure;
        }
    }

}