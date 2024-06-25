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
            Debug.Log("Flight");
        }

        protected override void OnStop()
        {
            try
            {
            }
            catch
            {

            }
        }

        protected override State OnUpdate()
        {
            try
            {
                if ((!_stateMachine.IsKnockback && _stateMachine.IsGrounded) || _stateMachine.IsDead)
                {
                    Debug.Log("Flight Fail");
                    Debug.Log($"_stateMachine.IsKnockback = {_stateMachine.IsKnockback} \\ _stateMachine.IsGrounded =  {_stateMachine.IsGrounded}");
                    _stateMachine.IsKnockback = false;
                    _stateMachine.IsDamaged = false;
                    return State.Failure;
                }
                // 일단 판정을 받아야함.
                if (_stateMachine.IsKnockback)
                {
                    Debug.Log("Flight Success");
                    return State.Success;
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