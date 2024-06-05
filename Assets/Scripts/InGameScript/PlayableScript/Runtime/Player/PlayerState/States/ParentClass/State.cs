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
            if(StateMachine.IsKnockback)
            {
                Debug.Log("Next State?");
                StateMachine.ChangeState(StateMachine.knockbackState);
                return;
            }
            if (StateMachine.IsDamaged && !StateMachine.IsGuard)
            {
                // 공격을 받았을 때, hitState로 변경해 줌.
                StateMachine.ChangeState(StateMachine.hitState);
                return;
            }
        }

        public virtual void Exit()
        {

        }

        protected void Movement(float speedSpeed)
        {
            if (StateMachine.physics.velocity.x <= speedSpeed && StateMachine.physics.velocity.x >= -speedSpeed)
            {
                // 바로 앞에 적이 있으면 더이상 이동하지 않음(애니메이션은 재생)
                // 머리와 다리쪽에서 Ray를 쏠 예정
                StateMachine.physics.velocity += Vector3.right * (StateMachine.moveAction.ReadValue<float>());
                
                Vector3 left = new Vector3((StateMachine.transform.position + Vector3.right).x + 2f,
                    StateMachine.transform.position.y, StateMachine.transform.position.z);
                Vector3 right = new Vector3((StateMachine.transform.position + Vector3.left).x - 2f,
                    StateMachine.transform.position.y, StateMachine.transform.position.z);
                if(StateMachine.moveAction.ReadValue<float>() < 0)
                    StateMachine.transform.LookAt(right);
                if (StateMachine.moveAction.ReadValue<float>() > 0)
                    StateMachine.transform.LookAt(left);
                //Vector3 euler = StateMachine.transform.GetChild(1).GetComponent<RectTransform>().localRotation.eulerAngles;
                //Debug.Log($"{euler} / {StateMachine.transform.GetChild(1).GetComponent<RectTransform>().transform.name}");
                //StateMachine.transform.GetChild(1).GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, StateMachine.moveAction.ReadValue<float>() < 0 ? 90 : -90, 0);
            }
            if (StateMachine.moveAction.ReadValue<float>() == 0)
                StateMachine.StandingVelocity();
        }
    }
}