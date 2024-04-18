using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBehaviourTree
{
    public class BehaviourTreeRunnder : MonoBehaviour
    {
        BehaviourTree tree;

        private void Start()
        {
            tree = new BehaviourTree();

            var log = new DebugLogNode();
            log.Message = "Hello World";


            var repeatNode = new RepeatNode();
            repeatNode.child = log;

            tree.rootNode = repeatNode;
        }

        private void Update()
        {
            tree.Update();   
        }
    }
}
