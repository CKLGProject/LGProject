using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class GuardNode : ActionNode
    {
        public AIAgent Agent;
        //private float _curTimer;
        //[SerializeField] private float maxGuardTimer;

        // 낮은 확률로 방어 해제 
        // 상대와 거리가 멀리 떨어지면 바로 해제
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
            Agent.GetStateMachine.isGuard = true;
            Agent.GetStateMachine.guardEffect.SetActive(true);
            Agent.effectManager.Play(EffectManager.EFFECT.Guard);
        }

        protected override void OnStop()
        {
            Agent.GetStateMachine.guardEffect.SetActive(false);
            Agent.effectManager.Stop(EffectManager.EFFECT.Guard);
            
        }

        // 키를 누르고 있는가? > 이 는 곧 플레이어가 가까이 있을 때,
        // 이는 가중치를 사용하거나 확률 싸움을 해야할 듯?
        // 게이지가 다 닳지 않았는가?
        // 그럼 내부에 타이머가 존재하겠네?
        protected override State OnUpdate()
        {
            if(Agent.GetStateMachine.isGuard)
            {
                int rand = Random.Range(1, 100);
                // 0퍼센트면 return failure;
                if (rand < percent || guardEnableRange < Vector3.Distance(Agent.transform.position, Agent.player.position))
                {
                    Agent.GetStateMachine.isGuard = false;
                    return State.Success;
                }
                return State.Running;
            }
            // 가드를 올리고 있는 상태 -> 흠 이건 잘 모르겠다...
            return State.Success;
        }

        // 가드 중에는 Running 아무것도 못하는 상태이며 타이머가 줄거나 랜덤으로 푸는 경우가 있다.
        private void AAA()
        {

        }
    }

}