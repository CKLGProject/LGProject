using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DownNode : ActionNode
    {
        public AIAgent Agent;
        [Space(10f)]
        public float downTimer;
        private float curTimer;
        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
            curTimer = 0;
            Agent.effectManager.PlayOneShot(EffectManager.EFFECT.Knockback);
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (Agent.GetStateMachine.isGrounded)
            {

                curTimer += Time.deltaTime;
                // 고정된 누어있는 시간이 존재함.
                if (downTimer < curTimer)
                {
                    // 누어있는 시간이 끝나면 Idle 상태가 되면서 일어남.
                    return State.Success;
                }
            }
            else
            {
                // 공중에 뜬 상태에서는 아무고토 모태
                curTimer = 0;
            }

            return State.Running;
        }
    }

}