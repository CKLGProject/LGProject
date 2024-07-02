using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class ActionWaitNode : ActionNode
    {
        public AIAgent Agent;
        [Space(10f)]
        private float curTimer = 0;
        public float judgmentTime = 0;
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
            // 시간 내에 공격 판정 범위 내에 들어오면 OK
            curTimer += Time.deltaTime;
            
            if(judgmentTime > curTimer)
            {
                // Boolean type Method Running 중 
                return State.Running;
            }
            return State.Failure;
        }
    }
}