using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AIBehaviourTree
{
    public class BehaviourTree
    {
        public Node rootNode;
        public Node.State treeState = Node.State.Running;


        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running)
            {
                treeState = rootNode.Update();

            }
            return treeState;
        }
    }
}
