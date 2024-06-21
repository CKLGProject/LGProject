using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class DownNode : ActionNode
    {
        public AIAgent Agent => AIAgent.Instance;
        private LGProject.PlayerState.PlayerStateMachine StateMachine => Agent.GetStateMachine;
       
        [Space(10f)]
        public float downTimer;
        private float _currentTimer;
        
        private static readonly int WakeUp = Animator.StringToHash("WakeUp");
        private static readonly int Run = Animator.StringToHash("Run");
      
        protected override void OnStart()
        {
            _currentTimer = 0;
            StateMachine.IsDamaged = false;
            StateMachine.IsDown = true;
            StateMachine.IsGrounded = true;
            StateMachine.animator.SetInteger(Run, 0);
            Debug.Log("Down");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            try
            {
                if (StateMachine.IsGrounded)
                {
                    _currentTimer += Time.deltaTime;
                    // 고정된 누어있는 시간이 존재함.
                    if (StateMachine.playable.DownWaitDelay < _currentTimer)
                    {
                        // 누어있는 시간이 끝나면 Idle 상태가 되면서 일어남.
                        StateMachine.animator.SetTrigger(WakeUp);
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