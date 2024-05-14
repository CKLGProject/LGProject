using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.BossAI
{
    public enum ArmType
    {
        LEFT = 1,
        RIGHT = 2,
    }
    public class AiStateMachine
    {
        public Transform Transform;
        public Animator[] Animators;
        public Vector3[] OriginPos;


        private PlayerState.Playable[] players;
        // List안에 Stock을 쌓는다.
        // 여기서 State Machine을 세팅해주기
        private IdleState _idleState;
        private AttackState _attackState;
        private AngryAttackState _angryAttackState;

        private BossPrincipal _boss;

        private State[] _stateArray;
        private int current = 0;

        public State CurrentState;

        public static AiStateMachine CreateAIStateMachine(GameObject _obj)
        {
            AiStateMachine asm = new AiStateMachine();

            asm._boss = _obj.GetComponent<BossPrincipal>();

            asm._idleState = new IdleState(asm, 1f);
            asm._attackState = new AttackState(asm, asm._boss.MoveWaitTime, asm._boss.AttackDelayTime, asm._boss.MoveSpeed);
            asm._angryAttackState = new AngryAttackState(asm, 1f);

            asm.Transform = _obj.transform;
            asm.Animators = new Animator[asm.Transform.childCount];
            asm.OriginPos = new Vector3[asm.Transform.childCount];

            for (int i = 0; i < asm.Transform.childCount; i++)
            {
                asm.Animators[i] = asm.Transform.GetChild(i).GetComponent<Animator>();
                // 시작 위치
                asm.OriginPos[i] = asm.Transform.GetChild(i).position;
            }
            return asm;
        }

        public void Initalize(State _startingState)
        {
            CurrentState = _startingState;
            _startingState.Enter();
        }

        public void InitStateArray(CURRENTSTATE[] _states)
        {
            _stateArray = new State[_states.Length];
            for(int i = 0; i < _states.Length; i++)
            {
                _stateArray[i] = GetState(_states[i]);
            }

            if (_stateArray.Length > 0)
                Initalize(_stateArray[0]);
        }

        public PlayerState.Playable[] PlayersProperties
        {
            get
            {
                return players;
            }
            set
            {
                players = value;
            }
        }
        
        public void MoveArm(Vector3 _target, float _speed, ArmType _armType )
        {
            Transform handTransform = Animators[(int)(_armType)].transform;
            Vector3 vec = new Vector3(_target.x, handTransform.position.y, handTransform.position.z);
            if(_armType == ArmType.RIGHT ? _target.x <= 0 : _target.x >= 0)
                handTransform.position = Vector3.MoveTowards(handTransform.position, vec, _speed * Time.deltaTime);

        }
        
        //public void SetPlayers(PlayerState.Playable[] players)
        //{
        //    Players = players;
        //}

        //public PlayerState.Playable[] GetPlayers()
        //{
        //    return Players;
        //}

        private State GetState(CURRENTSTATE _stateName)
        {
            switch (_stateName)
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

        public void ChangeState(State _changeState)
        {
            CurrentState.Exit();
            CurrentState = _changeState;
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

        /*********************************************
         * Animation Methods
         * **/
        public void SetAnimationTrigger(string _triggerName = "")
        {
            foreach (var anim in Animators)
            {
                anim.SetTrigger(_triggerName);
            }
        }

        public void SetAnimationTrigger(string _triggerName, int hands)
        {
            
        }

        public void SetAnimationBoolean(string _triggerName, bool _bParam = true)
        {
            foreach(var anim in Animators)
            {
                anim.SetBool(_triggerName, _bParam);
            }
        }

        public ArmType GetArmType(Vector3 startPoint)
        {
            return startPoint.x < 0 ? ArmType.RIGHT : ArmType.LEFT;
        }


    }
}
