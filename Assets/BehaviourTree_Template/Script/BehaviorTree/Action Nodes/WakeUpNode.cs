using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class WakeUpNode : ActionNode
    {
        public AIAgent Agent;
        private LGProject.PlayerState.PlayerStateMachine stateMachine;
        [Space(10f)]
        public float wakeUpTime;
        private float curTimer;
        protected override void OnStart()
        {
            //Debug.Log("Wake Up");
            curTimer = 0;
            if (stateMachine == null)
                stateMachine = AIAgent.Instance.GetStateMachine;
            stateMachine.IsKnockback = false;
            stateMachine.IsDamaged = false;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            curTimer += Time.deltaTime;
            // 일어나는 중이면 무적 판정
            if (wakeUpTime < curTimer )
            {
                stateMachine.animator.SetTrigger("WakeUp");
                Debug.Log("WakeUp");
                return State.Success;
            }
            if (stateMachine.IsDamaged)
                return State.Failure;
            return State.Running;
        }
    }

}