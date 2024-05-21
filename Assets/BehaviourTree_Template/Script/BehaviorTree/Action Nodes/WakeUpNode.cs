using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class WakeUpNode : ActionNode
    {
        public AIAgent Agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        [Space(10f)]
        public float wakeUpTime;
        private float curTimer;
        protected override void OnStart()
        {
            //Debug.Log("WakeUpStateStart");
            curTimer = 0;
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            _stateMachine.IsKnockback = false;
            _stateMachine.IsDamaged = false;
        }

        protected override void OnStop()
        {
            //Debug.Log("WakeUpStateStop");
            _stateMachine.IsDown = false;
        }

        protected override State OnUpdate()
        {
            curTimer += Time.deltaTime;
            // 일어나는 중이면 무적 판정
            if (_stateMachine.playable.WakeUpDelay < curTimer )
            {
                return State.Success;
            }
            if (_stateMachine.IsDamaged)
                return State.Failure;
            return State.Running;
        }
    }

}