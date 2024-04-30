using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class DashAttackNode : ActionNode
    {
        public float judgTimer = 0;
        float curTimer;
        float aniDelay = 0;
        // 대쉬 어택.
        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        // Dash Attack을 할 때 이동 방향을 바라보며 공격을 해야함.
        protected override State OnUpdate()
        {


            return State.Success;   
        }
    }

}