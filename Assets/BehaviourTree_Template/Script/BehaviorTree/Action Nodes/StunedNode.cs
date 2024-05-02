using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class StunedNode : ActionNode
    {
        public AIAgent Agent;
        [Space(10f)]
        public float stunedTimer;
        private float curTiemr;

        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            // 경직 중 공격을 당하면 초기화
            if(Agent.GetStateMachine.isHit)
            {
                // 피격 모션 출력
                Agent.effectManager.PlayOneShot(EffectManager.EFFECT.Hit);
                Agent.GetStateMachine.isHit = false;
                curTiemr = 0;
            }
            curTiemr += Time.deltaTime;
            if(stunedTimer < curTiemr || !Agent.GetStateMachine.isGrounded || Agent.GetStateMachine.isHit)
            {
                // 나 피격 상태 끝났어!
                // 그런데 공중에 있냐 체크 해야함
                return State.Success;
            }
            return State.Running;
        }
    }

}