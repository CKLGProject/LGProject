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
        public DownState(PlayerStateMachine _stateMachine, ref float _delay) : base(_stateMachine)
        {
            _curTimer = 0;
            this._delay = _delay;
        }

        public override void Enter()
        {
            base.Enter();
            _curTimer = 0;
            stateMachine.IsDown = true;
        }

        public override void Exit()
        {
            base.Exit();
            stateMachine.IsKnockback = false;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (stateMachine.IsGrounded)
            {
                if (_flight)
                {
                    _flight = false;
                    stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Airborne);
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Knockback).Forget();
                }
                // 땅에 닿았을 때 누워있는 State
                _curTimer += Time.deltaTime;
                if (_curTimer >= _delay)
                {
                    // 원래는 WakeUp State
                    // Wake UP 중에는 공격을 받아도 무적임.
                    stateMachine.ChangeState(stateMachine.wakeUpState);
                    return;
                }
            }
            else
            {
                if(!_flight)
                {
                    _flight = true;
                    stateMachine.playable.effectManager.Play(EffectManager.EFFECT.Airborne).Forget();

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