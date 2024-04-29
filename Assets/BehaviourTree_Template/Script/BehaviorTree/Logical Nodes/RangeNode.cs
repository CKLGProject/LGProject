using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class RangeNode : ActionNode
    {
        // 거리를 계산하는 노드
        public float range;
        [Space(10f)]
        public bool Reverse;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            // 플레이어와의 거리를 비교하여 반환.
            // 플레이어의 경우 하나밖에 없기 때문에 agent에서 관리를 해줘야할까?
            // 일단 관리를 해주다가 추후 정적 클래스로 변경 하는 것으로 하자.
            float distance = Vector3.Distance(agent.transform.position, agent.player.position);
            
            // 플레이어와의 거리가 가까워야 무엇이든 할 수 있기 때문에 다음과 같이 설정.
            return distance <= range ? State.Success : State.Failure;
        }
    }

}