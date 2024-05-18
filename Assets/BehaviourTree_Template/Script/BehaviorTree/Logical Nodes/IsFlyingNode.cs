using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        }

        protected override void OnStop()
        {

        }

        // 공중에 있는지 체크하는 노드
        // 어디서 부터 들어왔냐에 따라 다음 진행할 노드가 달라짐.
        protected override State OnUpdate()
        {
            if(_stateMachine.IsKnockback)
            {
                return State.Success;
            }
            return State.Success;
        }
    }

}