using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LGProject.PlayerState
{

    public class PlayerScript : Playable
    {
        private void OnDrawGizmos()
        {
            // Attack Collider를 한 곳에 고정할 필요가 있음.
            try
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position - transform.up * 1f);


                Gizmos.color = Color.blue;
                //Vector3 right = Vector3.right * (stateMachine.moveAction.ReadValue<float>() >= 0 ? +1.5f : -1.5f);

                Vector3 center = transform.position + Vector3.up * 0.5f;
                Vector3 right = Vector3.right * (directionX == true ? 0.7f : -0.7f);
                Gizmos.DrawLine(transform.position, transform.position + right * 0.5f);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.75f), transform.position + (Vector3.up * 0.75f) + right * 0.5f);

                if (stateMachine.currentState.GetType() == typeof(PlayerState.AttackState))
                {
                    switch (stateMachine.attackCount - 1)
                    {
                        case 0:
                            Gizmos.color = Color.red;
                            break;
                        case 1:
                            Gizmos.color = Color.blue;
                            break;
                        case 2:
                            Gizmos.color = Color.yellow;
                            break;
                    }
                    Gizmos.DrawWireCube(center + right, Vector3.one * 0.5f);
                }
                else if (stateMachine.currentState.GetType() == typeof(PlayerState.DashAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.3f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(center + right, hitBoxSize );
                }
                else if (stateMachine.currentState.GetType() == typeof(PlayerState.JumpAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one * 0.7f;
                    hitBoxSize.x *= 1.5f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(center + right, hitBoxSize);
                }
            }
            catch
            {
                //Debug.Log("On Play");
            }
        }

        private void InitStates()
        {
            // ref을 쓰는 이유
            // 일반적으로 사용하면 복사생성자를 쓰기 때문에 메모리 누수가 일어날 수 있는데, ref을 사용하면 레퍼런스 주소값으로 전달하기 때문에 복사하여 메모리를 사용하는 불상사를 막을 수 있음
            // 그럼 Out을 쓰지 않는 이유?
            // 기본적으로 Out을 사용하면 매서드 내부에서 직접적인 선언 ex) (out int a)가 메서드로 들어갔을 때 a = ? 을 반드시 해줘야 한다.

            stateMachine = new PlayerStateMachine();
            stateMachine = PlayerStateMachine.CreateStateMachine(this.gameObject);

            idleState = new IdleState(stateMachine);
            moveState = new MoveState(stateMachine, ref dashSpeed, maximumSpeed);
            jumpState = new JumpState(stateMachine, ref jumpScale, maximumJump);

            attackState = new AttackState(stateMachine, ref comboDelay, ref aniDelay, ref movingAttack);

            jumpAttackState = new JumpAttackState(stateMachine, maximumSpeed);
            dashAttackState = new DashAttackState(stateMachine, ref dashAttackDelay);

            hitState = new HitState(stateMachine, 1f);
            guardState = new GuardState(stateMachine);

            downState = new DownState(stateMachine, 1f);

            landingState = new LandingState(stateMachine);

            stateMachine.Initalize(idleState);

            //clip1.frameRate;

        }

        void Start()
        {
            InitStates();
            InitEffectManager();

            effectManager.InitParticles();
            for(int i = 0; i < stateMachine.animator.runtimeAnimatorController.animationClips.Length; i++)
            {
                string name = stateMachine.animator.runtimeAnimatorController.animationClips[i].name;
                float time = stateMachine.animator.runtimeAnimatorController.animationClips[i].length;
                stateMachine.SetAnimPlayTime(name, time);
                //Debug.Log($"{ stateMachine.animator.runtimeAnimatorController.animationClips[i].name} / { stateMachine.animator.runtimeAnimatorController.animationClips[i].length}'s");
            }
        }

        private void FixedUpdate()
        {

        }

        void Update()
        {
            stateMachine.currentState.LogicUpdate();
            velocity = stateMachine.physics.velocity;
            PlatformCheck();
        }

    }

}