using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class TestRepeatNode : DecoratorNode
    {
        public int repeatCount = 0;
        private int curCount = 0;
        protected override void OnStart()
        {
            curCount = 0;
        }

        protected override void OnStop()
        {
        
        }

        protected override State OnUpdate()
        {
            switch (child.Update())
            {
                case State.Running:
                    break;
                case State.Failure:
                    return State.Failure;
                case State.Success:
                    curCount++;
                    break;
                default:
                    break;
            }
            return curCount == repeatCount ? State.Success : State.Running ;

        }
    }
}