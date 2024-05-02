using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    // 누워 있는 State
    public class DownState : State
    {
        private float _curTimer = 0;
        private float _delay = 0;
        private bool _flight = false;
        public DownState(PlayerStateMachine _stateMachine, float _delay) : base(_stateMachine)
        {
            _curTimer = 0;
            this._delay = _delay;
        }

        public override void Enter()
        {
            base.Enter();
            Debug.Log("Down");
            _curTimer = 0;
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.ResetAnimParameters();
            stateMachine.animator.SetTrigger("WakeUp");

        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.isGrounded)
            {
                if (_flight)
                {
                    _flight = false;
                    stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Airborne);
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Knockback);
                }
                // 땅에 닿았을 때 누워있는 State
                _curTimer += Time.deltaTime;
                if (_curTimer >= _delay)
                {
                    // 원래는 WakeUp State
                    // Wake UP 중에는 공격을 받아도 무적임.
                    stateMachine.ChangeState(stateMachine.playable.idleState);
                    return;
                }
            }
            else
            {
                if(!_flight)
                {
                    _flight = true;
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Airborne);
                }
                _curTimer = 0;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}