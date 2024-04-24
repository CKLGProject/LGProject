using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class Playable : MonoBehaviour
    {
        public IdleState idleState;
        public MoveState moveState;
        public JumpState jumpState;
        public AttackState attackState;
        public JumpAttackState jumpAttackState;
        public DashAttackState dashAttackState;
        public HitState hitState;
        public GuardState guardState;
        public DownState downState;


        public Vector3 velocity = Vector3.zero;
        public const int maximumJump = 2;
        public int maximumSpeed = 4;


        public float dashSpeed;
        public float jumpScale;
        public float hitDelay;
        public GameObject guardEffect;              // 가드 이펙트인데 오브젝트로 일단 표현.


        // 공격 관련 인스펙터 
        [Range(1f, 10f)]
        public float AttackDelay = 1f;
        public float comboDelay = 0;
        public float aniDelay = 0;
        public bool movingAttack = true;

        public LayerMask layer;

        // 공격 방향
        public bool directionX = false;


        protected PlayerStateMachine stateMachine;


        public PlayerStateMachine GetStateMachine
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
            Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
            distanceXZ.y = 0f; // y는 0으로 설정.
                               //Forward = origin;
                               // Create a float the represent our distance
            float Sy = distance.y;    // 세로 높이의 거리를 지정.
            float Sxz = distanceXZ.magnitude;

            // 속도 추가
            float Vxz = Sxz / time;
            float Vy = Sy / time + height * Mathf.Abs(Physics.gravity.y) * time;
            // 계산으로 인해 두 축의 초기 속도를 가지고 새로운 벡터를 만들 수 있음.
            Vector3 result = distanceXZ.normalized;
            result *= Vxz;
            result.y = Vy;
            return result;
            #endregion
        }

        private RaycastHit hit;

        public void PlatformCheck()
        {
            // 일단 여기에 넣어보자
            Ray ray = new Ray(transform.position, -transform.forward);

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, 1 << 6))
            {
                stateMachine.collider.isTrigger = false;
                stateMachine.isGrounded = true;
                stateMachine.isJumpGuard = false;
                stateMachine.jumpInCount = 0;
            }
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.transform.name == $"Platform")
        //    {

        //        Debug.Log("onPlayform");
        //    }
        //}

        private void OnCollisionExit(Collision collision)
        {
            if (collision.transform.name == $"Platform")
            {
                stateMachine.isGrounded = false;
                stateMachine.collider.isTrigger = true;
                Debug.Log("offPlayform");
            }
        }
    }

}