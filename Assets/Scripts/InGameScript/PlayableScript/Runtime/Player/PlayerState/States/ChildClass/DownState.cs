using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    // 누워 있는 State
    public class DownState : State
    {
        private float _currentTimer;
        private float _delay;
        private bool _flight;
        private static readonly int Landing = Animator.StringToHash("Down");

        public DownState(PlayerStateMachine stateMachine, ref float delay) : base(stateMachine)
        {
            _currentTimer = 0;
            _delay = delay;
        }

        public override void Enter()
        {
            base.Enter();
            _currentTimer = 0;
            StateMachine.IsDown = true;
            StateMachine.IsKnockback = false;
            StateMachine.IsDamaged = false;
            StateMachine.animator.SetInteger("Attack", 0);
            StateMachine.animator.SetTrigger(Landing);
            StateMachine.ResetVelocity();
            
            // 피격 당해서 땅에 쿵 되었을 때 쿵 FX 출력
            StateMachine.VocaFX.PlayVoca(EVocaType.Kung);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            //base.LogicUpdate();
            if (StateMachine.IsGrounded)
            {
                if (_flight)
                {
                    _flight = false;
                    StateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Airborne);
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Knockback).Forget();
                }
                // 땅에 닿았을 때 누워있는 State
                _currentTimer += Time.deltaTime;
                if (_currentTimer >= _delay)
                {
                    // 원래는 WakeUp State
                    // Wake UP 중에는 공격을 받아도 무적임.
                    StateMachine.ChangeState(StateMachine.wakeUpState);
                    return;
                }
            }
            else
            {
                if(!_flight)
                {
                    _flight = true;
                    StateMachine.playable.effectManager.Play(EffectManager.EFFECT.Airborne).Forget();

                }
                _currentTimer = 0;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }

}