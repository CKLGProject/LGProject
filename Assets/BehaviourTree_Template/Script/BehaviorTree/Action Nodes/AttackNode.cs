using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackNode : ActionNode
    {
        // 공격을 진행하는 노드 
        // 애니메이션을 실행함.
        int count = 0;
        protected override void OnStart()
        {
            // 애니메이션 실행
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            count++;
            return State.Failure;
        }


    }
}