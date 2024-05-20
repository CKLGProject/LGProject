using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class StunedNode : ActionNode
    {
        public AIAgent Agent;
        public LGProject.PlayerState.PlayerStateMachine _stateMachine;
        [Space(10f)]
        public float stunedTimer;
        private float curTiemr;

        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
            if (_stateMachine == null)
                _stateMachine = Agent.GetStateMachine;  
            _stateMachine.animator.SetFloat("Run", 0f);
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            // 경직 중 공격을 당하면 초기화
            if(Agent.GetStateMachine.IsDamaged && !Agent.GetStateMachine.IsKnockback)
            {
                // 피격 모션 출력
                //Agent.effectManager.PlayOneShot(EffectManager.EFFECT.Hit);
                Agent.GetStateMachine.IsDamaged = false;
                curTiemr = 0;
            }
            curTiemr += Time.deltaTime;
            //if(stunedTimer < curTiemr || (Agent.GetStateMachine.isKnockback && Agent.GetStateMachine.isHit))
            if(_stateMachine.playable.HitDelay < curTiemr || (Agent.GetStateMachine.IsKnockback ))
            {
                return State.Success;
            }
            return State.Running;
        }
    }

}