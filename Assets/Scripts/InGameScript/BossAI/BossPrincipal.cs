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

        public CURRENTSTATE[] States;
        public Animator Animator;
        private AiStateMachine _stateMachine;


        // Start is called before the first frame update
        void Start()
        {
            _stateMachine = AiStateMachine.CreateAIStateMachine(this.gameObject);
            _stateMachine.InitStateArray(States);
            //_stateMachine.Initalize(_stateMachine.CurrentState);
            //_stateMachine.
        }

        // Update is called once per frame
        void Update()
        {
            _stateMachine.CurrentState.Update();
        }
    }
}