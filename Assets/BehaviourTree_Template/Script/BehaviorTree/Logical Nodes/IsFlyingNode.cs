using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace BehaviourTree
{
    public class IsFlyingNode : ActionNode
    {
        public AIAgent Agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;

        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            // 플라잉 상태가 될 경우 0.15초간 체크 안해야할듯.
        }

        protected override void OnStop()
        {

        }

        // 공중에 있는지 체크하는 노드
        // 어디서 부터 들어왔냐에 따라 다음 진행할 노드가 달라짐.
        protected override State OnUpdate()
        {
            if(!_stateMachine.IsGrounded)
                return State.Success;
            return State.Running;
        }
    }

}