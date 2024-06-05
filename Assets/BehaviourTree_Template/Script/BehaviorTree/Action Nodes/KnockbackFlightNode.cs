using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class KnockbackFlightNode : ActionNode
    {
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;

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

            if (_stateMachine.IsDead)
            {
                _stateMachine.IsKnockback = false;
                _stateMachine.IsDamaged = false;
                //_stateMachine.animator.SetTrigger("Landing");
                return State.Failure;
            }
            // 일단 판정을 받아야함.
            if(!_stateMachine.IsKnockback)
            {
                return State.Success;
            }

            return State.Running;

        }
    }

}