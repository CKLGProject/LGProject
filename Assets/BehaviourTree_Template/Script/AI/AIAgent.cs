using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTree
{
    // 조건
    // -> 상대를 찾으면 바로 공격 
    // -> 그러나 상대에게 공격을 받고있는 상태라면 숨어야함.
    // -> 어떻게 숨어야 할까? 주변에 가장 가까운 은신처를 찾아야함.
    //
    //
    public class AIAgent : LGProject.PlayerState.Playable
    {


        public Transform player;

        //public Animator animator;

        [System.Obsolete]
        private void Awake()
        {
            Random.seed = System.DateTime.Now.Millisecond;
            stateMachine = new LGProject.PlayerState.PlayerStateMachine();
            stateMachine = LGProject.PlayerState.PlayerStateMachine.CreateStateMachine(this.gameObject);

            stateMachine.guardEffect = guardEffect;
            stateMachine.guardEffect.SetActive(false);
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            // 일단 여기에 넣어보자
            PlatformCheck();
        }


    }
}