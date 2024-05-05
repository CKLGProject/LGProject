using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public struct AttackAnimationProperties
    {
        public float FirstAttackDelay;
        public float SecondAttackDelay;
        public float ThirdAttackDelay;
        
        public float ComboDelay;
    }

    public class Playable : MonoBehaviour
    {

        public Animator Animator;

        protected Vector3 velocity = Vector3.zero;
        [Tooltip("최대 점프 횟수")]
        public int maximumJumpCount = 2;
        [Tooltip("대쉬 최대 속도")]
        public int maximumSpeed = 4;

        [Tooltip("이동할 때 증가하는 속도")]
        public float dashSpeed;
        [Tooltip("한 번 점프할 때의 높이")]
        public float jumpScale;


        // 공격 관련 인스펙터 
        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴")]
        public float FirstAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함.")]
        public float FirstAttackJudgeDelay = 1f;

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴")]
        public float SecondAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함.")]
        public float SecondAttackJudgeDelay = 1f;
        

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴")]
        public float ThridAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함.")]
        public float ThirdAttackJudgeDelay = 1f;


        [Range(0.0f, 1.0f), Tooltip("공격 후 Idle로 돌아오기 까지의 시간")]
        public float dashAttackDelay = 0;

        [Range(0.0f, 1.0f), Tooltip("피격 후 Idle로 돌아오기 까지의 시간")]
        public float hitDelay = 0;

        [Range(0.0f, 3.0f), Tooltip("다운 후 Idle로 돌아오기 까지의 시간")]
        public float wakeUpDelay = 0;

        public bool movingAttack = true;


        // 공격 방향
        [HideInInspector] public bool directionX = false;

        public EffectManager effectManager;

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

        protected void InitEffectManager()
        {
            effectManager = GetComponent<EffectManager>();
            if(effectManager == null)
            {
                Debug.LogError("EffectManager 없음");
            }
        }


        #region CheckFields
        private RaycastHit hit;

        float curTimer = 0;
        float downTimer = 0.5f;

        public void IsPushDownKey()
        {
            if (stateMachine.isDown)
            {
                curTimer += Time.deltaTime;
                if (downTimer < curTimer)
                {
                    stateMachine.isDown = false;
                    curTimer = 0;
                }
            }
        }

        public void NewPlatformCheck()
        {
            if(transform.position.y > 1)
            {

            }
        }

        public void PlatformCheck()
        {
            // 일단 여기에 넣어보자
            Ray ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);

            // 위를 체크하고 싶은데...
            if (!stateMachine.isDown)
            {
                if (Physics.Raycast(ray, out hit, 0.3f, 1 << 6))
                {
                    if (!stateMachine.isGrounded)
                    {
                        effectManager.Play(EffectManager.EFFECT.Landing);
                        stateMachine.collider.isTrigger = false;
                        stateMachine.isGrounded = true;
                        stateMachine.isJumpGuard = false;
                        stateMachine.jumpInCount = 0;
                        stateMachine.StandingVelocity();


                        // 이거 AI랑 공용으로 사용중이라 나중에 안되게 해야함.
                        if(!stateMachine.isKnockback && stateMachine.currentState != null)
                            stateMachine.ChangeState(stateMachine.landingState);

                        //stateMachine.isHit = false;
                    }
                }
                else
                {
                    stateMachine.isGrounded = false;
                    stateMachine.collider.isTrigger = true;
                    stateMachine.animator.SetBool("Flying",stateMachine.jumpInCount < 1 ? true : false);
                    if(stateMachine.jumpInCount < 1)
                        stateMachine.jumpInCount++;
                }
            }
        }
        #endregion
    }

}