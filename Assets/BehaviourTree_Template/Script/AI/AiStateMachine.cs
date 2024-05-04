using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.BossAI
{
    public class AiStateMachine
    {
        public Transform Transform;
        public Animator Animator;

        //public 
        // List안에 Stock을 쌓는다.
        // 여기서 State Machine을 세팅해주기
        private IdleState _idleState;
        private AttackState _attackState;
        private AngryAttackState _angryAttackState;

        private State[] _stateArray;
        private int current = 0;

        public State CurrentState;

        public static AiStateMachine CreateAIStateMachine(GameObject obj)
        {
            AiStateMachine asm = new AiStateMachine();
            
            asm._idleState = new IdleState(asm, 1f);
            asm._attackState = new AttackState(asm, 1f);
            asm._angryAttackState = new AngryAttackState(asm, 1f);

            asm.Transform = obj.transform;
            //asm.Animator = obj.GetComponent<AiStateMachine>().Animator;
            asm.Initalize(asm._idleState);
            //asm.InitStateArray();

            return asm;
        }

        public void Initalize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void InitStateArray(CURRENTSTATE[] states)
        {
            _stateArray = new State[states.Length];

            for(int i = 0; i < states.Length; i++)
            {
                _stateArray[i] = GetState(states[i]);
            }

        }
        private State GetState(CURRENTSTATE stateName)
        {
            switch (stateName)
            {
                case CURRENTSTATE.IDLE:
                    return _idleState;
                case CURRENTSTATE.ATTACK:
                    return _attackState;
                case CURRENTSTATE.ANGRY:
                    return _idleState;
                case CURRENTSTATE.ANGRYATTACK:
                    return _angryAttackState;
            }
            return null;
        }

        public void ChangeState(State changeState)
        {
            CurrentState.Exit();
            CurrentState = changeState;
            CurrentState.Enter();
        }

        public void NextState()
        {
            CurrentState.Exit();
            current += 1;
            if (current >= _stateArray.Length)
                current = 0;
            CurrentState = _stateArray[current];
            CurrentState.Enter();
        }

    }
}
