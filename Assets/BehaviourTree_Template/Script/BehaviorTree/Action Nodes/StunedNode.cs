using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class StunedNode : ActionNode
    {
        public AIAgent Agent => AIAgent.Instance;

        [Space(10f)]
        public float stunedTimer;

        private float _currentTimer;

        private LGProject.PlayerState.PlayerStateMachine StateMachine => Agent.GetStateMachine;

        protected override void OnStart()
        {
            StateMachine.IsGuard = false;
            //StateMachine.IsDamaged = false;
            StateMachine.animator.ResetTrigger("Landing");
            StateMachine.animator.ResetTrigger("Knockback");
            StateMachine.animator.ResetTrigger("WakeUp");
            StateMachine.animator.SetFloat("Run", 0f);
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            try
            {
                // 경직 중 공격을 당하면 초기화
                if (Agent.GetStateMachine.IsDamaged && !Agent.GetStateMachine.IsKnockback)
                {
                    // 피격 모션 출력
                    Agent.GetStateMachine.IsDamaged = false;
                    _currentTimer = 0;
                }

                _currentTimer += Time.deltaTime;

                if ((Agent.GetStateMachine.IsKnockback))
                {
                    return State.Success;
                }
                else if(StateMachine.playable.HitDelay < _currentTimer)
                {
                    Agent.GetStateMachine.IsDamaged = false;
                    return State.Failure;
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