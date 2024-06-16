using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DownNode : ActionNode
    {
        public AIAgent Agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        [Space(10f)]
        public float downTimer;
        private float _curTimer;
        private static readonly int WakeUp = Animator.StringToHash("WakeUp");
        private static readonly int Run = Animator.StringToHash("Run");
        protected override void OnStart()
        {
            if (Agent == null)
                Agent = AIAgent.Instance;
            if (_stateMachine == null)
                _stateMachine = Agent.GetStateMachine;
            _curTimer = 0;
            _stateMachine.IsDamaged = false;
            _stateMachine.IsDown = true;
            _stateMachine.IsGrounded = true;
            _stateMachine.animator.SetInteger(Run, 0);
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            try
            {
                if (_stateMachine.IsGrounded)
                {
                    _curTimer += Time.deltaTime;
                    // 고정된 누어있는 시간이 존재함.
                    if (_stateMachine.playable.DownWaitDelay < _curTimer)
                    {
                        // 누어있는 시간이 끝나면 Idle 상태가 되면서 일어남.
                        _stateMachine.animator.SetTrigger(WakeUp);
                        return State.Success;
                    }
                }
                return State.Running;
            }
            catch
            {
                return State.Failure;
            }
        }
    }

}