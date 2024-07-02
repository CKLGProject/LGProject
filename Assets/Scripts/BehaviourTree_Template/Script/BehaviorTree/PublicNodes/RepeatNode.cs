using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class RepeatNode : DecoratorNode
    {
        public bool Loop = true;
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }


        protected override State OnUpdate()
        {
            if (Loop)
            {
                child.Update();
                return State.Running;
            }
            else
            {
                switch (child.Update())
                {
                    case State.Running:
                        break;
                    case State.Failure:
                        return State.Failure;
                    case State.Success:
                        break;
                    default:
                        break;
                }
                return State.Running;
            }
        }
    }
}
