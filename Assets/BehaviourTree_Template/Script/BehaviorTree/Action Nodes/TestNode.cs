using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class TestNode : ActionNode
    {
        protected override void OnStart()
        {
            Debug.Log("AA");
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {

            return State.Success;
        }
    }

}