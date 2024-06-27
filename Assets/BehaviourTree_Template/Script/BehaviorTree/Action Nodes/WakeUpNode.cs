using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public class WakeUpNode : ActionNode
    {
        public AIAgent Agent;
        private LGProject.PlayerState.PlayerStateMachine _stateMachine;
        [Space(10f)]
        public float wakeUpTime;
        private float curTimer;

        private readonly int Landing = Animator.StringToHash("Landing");
        protected override void OnStart()
        {
            curTimer = 0;
            if (_stateMachine == null)
                _stateMachine = AIAgent.Instance.GetStateMachine;
            _stateMachine.IsKnockback = false;
            _stateMachine.IsDamaged = false;
            _stateMachine.animator.ResetTrigger(Landing);
            if (!_stateMachine.IsDead)
            {
                _stateMachine.BlinkPlayerMesh();
            }
            //_stateMachine.BlinkPlayerMaterial();
            //Debug.Log("WakeUp");
        }

        protected override void OnStop()
        {
            //Debug.Log("WakeUpStateStop");
            try
            {
                _stateMachine.IsDown = false;
                //Debug.Log("WakeUpEnd");
            }
            catch
            {

            }
        }

        protected override State OnUpdate()
        {
            try
            {
                if (_stateMachine.IsDead)
                    return State.Failure;
                curTimer += Time.deltaTime;
                // 일어나는 중이면 무적 판정
                if (_stateMachine.playable.WakeUpDelay < curTimer)
                {
                    return State.Success;
                }
                if (_stateMachine.IsDamaged)
                    return State.Failure;
                return State.Running;
            }
            catch
            {
                return State.Failure;
            }
        }
    }

}