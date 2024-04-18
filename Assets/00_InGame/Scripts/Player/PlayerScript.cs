using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : Playable 
{
    /// <summary>
    /// Player State
    /// </summary>
    //[Header("PlayerInspeco")]


    //PlayerState.PlayerStateMachine stateMachine;
   
    //public PlayerState.PlayerStateMachine GetStateMachine
    //{
    //    get
    //    {
    //        return stateMachine;
    //    }

    //}

    private void OnDrawGizmos()
    {
        // Attack Collider�� �� ���� ������ �ʿ䰡 ����.
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
        // ref�� ���� ����
        // �Ϲ������� ����ϸ� ��������ڸ� ���� ������ �޸� ������ �Ͼ �� �ִµ�, ref�� ����ϸ� ���۷��� �ּҰ����� �����ϱ� ������ �����Ͽ� �޸𸮸� ����ϴ� �һ�縦 ���� �� ����
        // �׷� Out�� ���� �ʴ� ����?
        // �⺻������ Out�� ����ϸ� �ż��� ���ο��� �������� ���� ex) (out int a)�� �޼���� ���� �� a = ? �� �ݵ�� ����� �Ѵ�.

        stateMachine = new PlayerState.PlayerStateMachine();
        stateMachine = PlayerState.PlayerStateMachine.CreateStateMachine(this.gameObject);

        idleState = new PlayerState.IdleState(stateMachine);
        moveState = new PlayerState.MoveState(stateMachine, ref dashSpeed, maximumSpeed);
        jumpState = new PlayerState.JumpState(stateMachine, ref jumpScale, maximumJump);

        attackState = new PlayerState.AttackState(stateMachine, ref comboDelay, ref aniDelay, ref movingAttack);

        jumpAttackState = new PlayerState.JumpAttackState(stateMachine, maximumSpeed);
        dashAttackState = new PlayerState.DashAttackState(stateMachine, ref aniDelay);

        hitState = new PlayerState.HitState(stateMachine, 1f);
        guardState = new PlayerState.GuardState(stateMachine, guardEffect);

        downState = new PlayerState.DownState(stateMachine, 1f);

        stateMachine.guardEffect = guardEffect;

        guardEffect.SetActive(false);

        //Instantiate(new GameObject(), transform.position + Vector3.down, Quaternion.identity);

        stateMachine.Initalize(idleState);
    }

    private void FixedUpdate()
    {

    }

    RaycastHit hit;

    void Update()
    {
        stateMachine.currentState.LogicUpdate();
        velocity = stateMachine.physics.velocity;
        //Attack = stateMachine.jumpAction.triggered;

        // �ϴ� ���⿡ �־��
        Ray ray = new Ray(transform.position, -transform.forward);
        
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, layer))
        {
            //Debug.Log("AA");
            stateMachine.collider.isTrigger = false;
            //Debug.Log("AA");
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.name == $"Platform")
        {
            stateMachine.isGrounded = true;
            stateMachine.isJumpGuard = false;
            stateMachine.jumpInCount = 0;
            //stateMachine.collider.isTrigger = false;
            //Debug.Log("Hit Ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.name == $"Platform")
        {
            stateMachine.isGrounded = false;
            stateMachine.collider.isTrigger = true;
            stateMachine.isJumpGuard = true;
            //Debug.Log("Jumping");
        }
    }
}
