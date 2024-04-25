using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BehaviourTree
{
    // 움직임을 판단. 상대를 향해 이동
    // 점프를 할 때도 있을 것이다. 이는 
    public class MoveNode : ActionNode
    {
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            return State.Failure;
        }
    }

}