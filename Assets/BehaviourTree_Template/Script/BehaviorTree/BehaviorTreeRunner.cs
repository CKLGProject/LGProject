using LGProject;
using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree Tree;
        public BehaviorTree[] Trees;

        private void Start()
        {
#if UNITY_EDITOR
            Tree = Tree.Clone();
            //tree.Bind(GetComponent<AIAgent>());
#endif

            //SpawnTheAIModelNPattern();
        }

        // 처음 생성되었을 때, 확률에 따라 출력될 모델의 종류를 결정하고 그에 맞는 세팅을 해준다.
        // GameManager를 통해 Play 횟수를 체크하고 3의 배수 마다 강한 AI를 출력시킨다.
        public void SpawnTheAIModelNPattern()
        {
            // AI Type 및 현재 몇 번째 플레이인지 체크하기
            //if (Singleton.Instance<GameManager>())
            // 3번마다 다른 AI를 장착
            bool HardMode = BattleSceneManager.Instance.PlayCount % 3 == 0;
            Tree = HardMode ? Trees[0] : Trees[1];
        }

        private void Update()
        {
            if(BattleSceneManager.Instance.IsStart)
                Tree.Update();
        }
    }
}