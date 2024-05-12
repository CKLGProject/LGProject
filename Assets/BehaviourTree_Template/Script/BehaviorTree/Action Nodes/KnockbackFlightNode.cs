using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class KnockbackFlightNode : ActionNode
    {
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        private float _curTimer;
        private float _WaitTime;
        protected override void OnStart()
        {
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            _stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Airborne).Forget();
            //AIAgent.Instance.GetStateMachine.
        }

        protected override void OnStop()
        {
            _stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Airborne);

        }

        protected override State OnUpdate()
        {

            // 일단 판정을 받아야함.
            if(!_stateMachine.IsKnockback)
            {
                _stateMachine.animator.SetTrigger("WakeUp");
                return State.Success;
            }

            return State.Running;

        }
    }

}