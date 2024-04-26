using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace BehaviourTree
{
    // 움직임을 판단. 상대를 향해 이동
    // 점프를 할 때도 있을 것이다. 이는 
    public class MoveNode : ActionNode
    {
        // 어디로 이동할 것인가가 관건
        // 점프는 별개의 영역이므로 이동자체의 로직만 생각하자.
        // 목표 지점을 두는 것은 어떨까?
        // 내가 이동할 목적지가 존재하면 나름 알고리즘에서 제약이 없을 것 같다.
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