using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LGProject.PlayerState;

namespace BehaviourTree
{
    public class GuardNode : ActionNode
    {
        private PlayerStateMachine stateMachine;
        public AIAgent Agent;
        //private float _curTimer;
        //[SerializeField] private float maxGuardTimer;

        // 낮은 확률로 방어 해제 
        // 상대와 거리가 멀리 떨어지면 바로 해제
        private float curTimer;
        [SerializeField] private float onTimer;
        [SerializeField,Range(0, 100)] private int percent = 0;
        [SerializeField] private float guardEnableRange;

        //public float 
        // 회복도 해야겠네? 생각해야할 것이 많음 좀 우선순위를 낮추자.
        protected override void OnStart()
        {
            // 애니메이션 재생
            // 현재 가드를 올렸따!
            if (Agent == null)
                Agent = AIAgent.Instance;
            if (stateMachine == null)
                stateMachine = AIAgent.Instance.GetStateMachine;
            stateMachine.IsGuard = true;
            stateMachine.playable.effectManager.PlayOneShot(EffectManager.EFFECT.Guard);
            curTimer = 0;
            stateMachine.animator.SetTrigger("Guard");
            stateMachine.animator.SetFloat("Run", 0);
        }

        protected override void OnStop()
        {
            stateMachine.playable.effectManager.Stop(EffectManager.EFFECT.Guard);

        }

        // 키를 누르고 있는가? > 이 는 곧 플레이어가 가까이 있을 때,
        // 이는 가중치를 사용하거나 확률 싸움을 해야할 듯?
        // 게이지가 다 닳지 않았는가?
        // 그럼 내부에 타이머가 존재하겠네?
        protected override State OnUpdate()
        {
            if(stateMachine.IsDamaged)
            {
                stateMachine.IsDamaged = false;
                curTimer = 0;
            }    
            if(stateMachine.IsGuard)
            {
                curTimer += Time.deltaTime;
                if(curTimer > onTimer)
                {
                    int rand = Random.Range(1, 100);
                    // 0퍼센트면 return failure;
                    if (rand < percent || guardEnableRange < Vector3.Distance(stateMachine.transform.position, Agent.player.position))
                    {
                        stateMachine.IsGuard = false;
                        stateMachine.animator.SetTrigger("Idle");
                        return State.Success;
                    }
                }
                return State.Running;
            }
            return State.Success;
        }

        // 가드 중에는 Running 아무것도 못하는 상태이며 타이머가 줄거나 랜덤으로 푸는 경우가 있다.
        private void AAA()
        {

        }
    }

}