using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class GuardNode : ActionNode
    {
        public AIAgent Agent;
        //public float 
        // 회복도 해야겠네? 생각해야할 것이 많음 좀 우선순위를 낮추자.
        protected override void OnStart()
        {
            // 애니메이션 재생
            // 현재 가드를 올렸따!
            if (Agent == null)
                Agent = AIAgent.Instance;
            Agent.GetStateMachine.isGuard = Agent.GetStateMachine.isGuard ? false : true;
            Agent.GetStateMachine.guardEffect.SetActive(Agent.GetStateMachine.isGuard ? true : false);
            //agent.GetStateMachine.guardEffect.
        }

        protected override void OnStop()
        {

        }

        // 키를 누르고 있는가? > 이 는 곧 플레이어가 가까이 있을 때,
        // 이는 가중치를 사용하거나 확률 싸움을 해야할 듯?
        // 게이지가 다 닳지 않았는가?
        // 그럼 내부에 타이머가 존재하겠네?
        protected override State OnUpdate()
        {
            
            // 가드를 올리고 있는 상태 -> 흠 이건 잘 모르겠다...
            return State.Success;
        }
    }

}