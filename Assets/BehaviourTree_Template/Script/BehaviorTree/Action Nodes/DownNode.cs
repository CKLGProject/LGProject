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
        private float curTimer;
        private static readonly int Landing = Animator.StringToHash("Landing");
        private static readonly int WakeUp = Animator.StringToHash("WakeUp");
        protected override void OnStart()
        {
            //Debug.Log("DownNodeStart");
            if (Agent == null)
                Agent = AIAgent.Instance;
            if (_stateMachine == null)
                _stateMachine = Agent.GetStateMachine;
            curTimer = 0;
            _stateMachine.animator.SetTrigger(Landing);
            //Agent.effectManager.Play(EffectManager.EFFECT.Knockback).Forget();
            _stateMachine.IsDamaged = false;
            _stateMachine.IsDown = true;
            _stateMachine.IsGrounded = true;
            //Debug.Log("DownNode");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (_stateMachine.IsGrounded)
            {
                curTimer += Time.deltaTime;
                // 고정된 누어있는 시간이 존재함.
                if (_stateMachine.playable.DownWaitDelay < curTimer)
                {
                    // 누어있는 시간이 끝나면 Idle 상태가 되면서 일어남.
                    _stateMachine.animator.SetTrigger(WakeUp);
                    return State.Success;
                }
            }
            return State.Running;
        }
    }

}