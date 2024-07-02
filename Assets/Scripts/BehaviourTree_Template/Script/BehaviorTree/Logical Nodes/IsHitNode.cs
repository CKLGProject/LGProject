using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{

    // 데미지를 입히는 노드 
    public class IsHitNode : ActionNode
    {
        //public AIAgent Agent;
        protected override void OnStart()
        {
             
        }

        protected override void OnStop()
        {

        }

        // 모든 행동에서 최상위에 존재하는 노드
        // 일단 쳐맞았기 때문에 경직상태가 되면서 다른 행동은 불가능하게 됨.
        protected override State OnUpdate()
        {
            return State.Failure;
        }
    }
}
