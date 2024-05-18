using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class AttackNode : ActionNode
    {
        public AIAgent Agent;
        // 공격을 진행하는 노드 
        // 애니메이션을 실행함.
        //int count = 0;
        protected override void OnStart()
        {
            // 애니메이션 실행
            if (Agent == null)
                Agent = AIAgent.Instance;
        }

        protected override void OnStop()
        {
            Agent.GetStateMachine.IsNormalAttack = false;

        }

        protected override State OnUpdate()
        {
            // 공격을 했으니 이제 뭔가를 여기서 해야하는데...
            return State.Success;
        }
    }
}