using UnityEngine;

namespace LGProject.PlayerState
{
    public abstract class State
    {
        protected readonly PlayerStateMachine StateMachine;

        private RaycastHit _hit;

        // 키 입력에 다른 Action을 출력

        public State(PlayerStateMachine stateMachine) 
        {
            StateMachine = stateMachine;
        }

        public virtual void Enter()
        {
            //Debug.Log($"Enter State = {this.ToString()}");
        }

        public virtual void PhysicsUpdate()
        {
        }

        public virtual void LogicUpdate()
        {
            if(StateMachine.IsDamaged && !StateMachine.IsGuard)
            {
                // 공격을 받았을 때, hitState로 변경해 줌.
                //StateMachine.physics.velocity = Vector3.zero;
                StateMachine.ChangeState(StateMachine.hitState);
            }
        }

        public virtual  void Exit()
        {

        }

    }

}