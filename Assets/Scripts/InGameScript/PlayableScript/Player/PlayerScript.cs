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
                Gizmos.color = Color.blue;
                //Vector3 right = Vector3.right * (stateMachine.moveAction.ReadValue<float>() >= 0 ? +1.5f : -1.5f);
                Vector3 right = Vector3.right * (directionX == true ? 1 : -1);

                Gizmos.DrawLine(transform.position + (Vector3.down * 0.9f), transform.position + (Vector3.down * 0.9f) + right);
                Gizmos.DrawLine(transform.position + (Vector3.up * 0.9f), transform.position + (Vector3.up * 0.9f) + right);

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
                    Gizmos.DrawWireCube(transform.position + right, Vector3.one);
                }
                else if (stateMachine.currentState.GetType() == typeof(PlayerState.DashAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one;
                    hitBoxSize.x *= 1.3f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
                else if (stateMachine.currentState.GetType() == typeof(PlayerState.JumpAttackState))
                {
                    Gizmos.color = Color.red;
                    Vector3 hitBoxSize = Vector3.one;
                    hitBoxSize.x *= 1.5f;
                    //hitBoxSiz
                    Gizmos.DrawWireCube(transform.position + right, hitBoxSize);
                }
            }
            catch
            {
                //Debug.Log("On Play");
            }
        }


        void Start()
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
            dashAttackState = new DashAttackState(stateMachine, ref aniDelay);

            hitState = new HitState(stateMachine, 1f);
            guardState = new GuardState(stateMachine, guardEffect);

            downState = new DownState(stateMachine, 1f);

            stateMachine.guardEffect = guardEffect;

            guardEffect.SetActive(false);

            //Instantiate(new GameObject(), transform.position + Vector3.down, Quaternion.identity);

            stateMachine.Initalize(idleState);
        }

        private void FixedUpdate()
        {

        }

        void Update()
        {
            stateMachine.currentState.LogicUpdate();
            velocity = stateMachine.physics.velocity;
            //Attack = stateMachine.jumpAction.triggered;
            PlatformCheck();


        }

    }

}