using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.BossAI
{
    public enum CURRENTSTATE
    {
        IDLE,
        ATTACK,
        ANGRY,
        ANGRYATTACK
    }
    public class BossPrincipal : MonoBehaviour
    {

        public float MoveWaitTime = 0f;
        public float AttackDelayTime = 0;
        public float MoveSpeed = 2f;
        //public float 

        public CURRENTSTATE[] States;
        public PlayerState.Playable[] playableCharacters;

        private AiStateMachine _stateMachine;

        [SerializeField] private bool angry; 


        // Start is called before the first frame update
        void Start()
        {
            _stateMachine = AiStateMachine.CreateAIStateMachine(this.gameObject);
            _stateMachine.InitStateArray(States);
            _stateMachine.PlayersProperties = playableCharacters;
            //_stateMachine.

        }

        // Update is called once per frame
        void Update()
        {
            if(_stateMachine.CurrentState != null)
                _stateMachine.CurrentState.Update();

        }

    }
}