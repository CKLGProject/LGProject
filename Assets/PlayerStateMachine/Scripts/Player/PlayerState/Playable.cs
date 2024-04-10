using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : MonoBehaviour
{
    public PlayerState.IdleState idleState;
    public PlayerState.MoveState moveState;
    public PlayerState.JumpState jumpState;
    public PlayerState.AttackState attackState;
    public PlayerState.JumpAttackState jumpAttackState;
    public PlayerState.DashAttackState dashAttackState;
    public PlayerState.HitState hitState;
    public PlayerState.GuardState guardState;
    public PlayerState.DownState downState;


    public Vector3 velocity = Vector3.zero;
    public const int maximumJump = 2;
    public int maximumSpeed = 4;


    public float dashSpeed;
    public float jumpScale;
    public float hitDelay;
    public GameObject guardEffect;              // ���� ����Ʈ�ε� ������Ʈ�� �ϴ� ǥ��.


    // ���� ���� �ν����� 
    [Range(1f, 10f)]
    public float AttackDelay = 1f;
    public float comboDelay = 0;
    public float aniDelay = 0;
    public bool movingAttack = true;

    public LayerMask layer;

    // ���� ����
    public bool directionX = false;


    protected PlayerState.PlayerStateMachine stateMachine;


    public PlayerState.PlayerStateMachine GetStateMachine
    {
        get
        {
            return stateMachine;
        }

    }
    public Vector3 CaculateVelocity(Vector3 target, Vector3 origin, float time, float height = 1.5f)
    {
        #region Omit
        // define the distance x and y first;
        Vector3 distance = target - origin;
        Vector3 distanceXZ = distance; // x�� z�� ����̸� �⺻������ �Ÿ��� ���� ����.
        distanceXZ.y = 0f; // y�� 0���� ����.
                           //Forward = origin;
                           // Create a float the represent our distance
        float Sy = distance.y;    // ���� ������ �Ÿ��� ����.
        float Sxz = distanceXZ.magnitude;

        // �ӵ� �߰�
        float Vxz = Sxz / time;
        float Vy = Sy / time + height * Mathf.Abs(Physics.gravity.y) * time;
        // ������� ���� �� ���� �ʱ� �ӵ��� ������ ���ο� ���͸� ���� �� ����.
        Vector3 result = distanceXZ.normalized;
        result *= Vxz;
        result.y = Vy;
        return result;
        #endregion
    }
}
