using LGProject;
using LGProject.PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class BehaviorTreeRunner : MonoBehaviour
    {
        public BehaviorTree tree;


        private void Start()
        {
#if UNITY_EDITOR
            tree = tree.Clone();
            //tree.Bind(GetComponent<AIAgent>());
#endif
            
        }

        private void Update()
        {
            if(BattleSceneManager.Instance.IsStart)
                tree.Update();
        }
    }
}