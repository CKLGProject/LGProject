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
            Debug.Log("Stuned");
            if (Agent == null)
                Agent = AIAgent.Instance;
            Agent.effectManager.Play(EffectManager.EFFECT.Hit);
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
                //Debug.Log("아야!");
                Agent.effectManager.Play(EffectManager.EFFECT.Hit);
                Agent.GetStateMachine.isHit = false;
                curTiemr = 0;
            }
            curTiemr += Time.deltaTime;
            if(stunedTimer < curTiemr)
            {
                // 나 피격 상태 끝났어!
                // 그런데 공중에 있냐 체크 해야함
                Agent.effectManager.Stop(EffectManager.EFFECT.Hit);
                return State.Success;
            }
            return State.Running;
        }
    }

}