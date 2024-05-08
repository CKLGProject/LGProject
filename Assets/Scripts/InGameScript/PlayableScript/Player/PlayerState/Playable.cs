using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using TMPro;
using Cysharp.Threading.Tasks;

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
        [Tooltip("최대 점프 횟수"), SerializeField, HideInInspector]
        public int MaximumJumpCount = 2;
        [Tooltip("대쉬 최대 속도"), SerializeField, HideInInspector]
        public int MaximumSpeed = 4;

        [Tooltip("이동할 때 증가하는 속도"), SerializeField, HideInInspector]
        public float DashSpeed = 0;
        [Tooltip("한 번 점프할 때의 높이"), SerializeField, HideInInspector]
        public float JumpScale = 5;


        // 공격 관련 인스펙터 
        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴"), SerializeField, HideInInspector]
        public float FirstAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float FirstAttackJudgeDelay = 1f;

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴")]
        public float SecondAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float SecondAttackJudgeDelay = 1f;
        

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴"), SerializeField, HideInInspector]
        public float ThirdAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float ThirdAttackJudgeDelay = 1f;


        [Range(0.0f, 1.0f), Tooltip("공격 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float dashAttackDelay = 0;

        [Range(0.0f, 1.0f), Tooltip("피격 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float hitDelay = 0;

        [Range(0.0f, 3.0f), Tooltip("다운 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float wakeUpDelay = 0;


        [HideInInspector] public bool movingAttack = true;

        public TextMeshProUGUI textGUI;
        public Vector3 AliveOffset;
        public float respawnTime;
        public float DeadLine;
        //public float damageGage = 0;

        // 공격 방향
        [HideInInspector] public bool directionX = false;
        [HideInInspector] public AnimationCurve jumpCurve;

        public Platform underPlatform;

        public EffectManager effectManager;

        float _gravity = -9.8f;
        float _groundedGravity = -0.05f;
        float initialJumpVelocity ;
        float maxJumpHeight = 1.5f;
        float maxJumpTime = 0.5f;





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

        public void PlatformCheck()
        {
            // 일단 여기에 넣어보자
            Ray ray = new Ray(transform.position + Vector3.up * 0.25f, Vector3.down);

            if (Physics.Raycast(ray, out hit, 0.3f, 1 << 6))
            {
                if (!stateMachine.isGrounded)
                {
                    effectManager.Play(EffectManager.EFFECT.Landing).Forget();
                    stateMachine.collider.isTrigger = false;
                    stateMachine.isGrounded = true;
                    stateMachine.isJumpGuard = false;
                    stateMachine.jumpInCount = 0;
                    stateMachine.StandingVelocity();


                    // 이거 AI랑 공용으로 사용중이라 나중에 안되게 해야함.
                    if (!stateMachine.isKnockback && stateMachine.currentState != null)
                        stateMachine.ChangeState(stateMachine.landingState);

                    //stateMachine.isHit = false;
                }
            }
            else
            {
                stateMachine.isGrounded = false;
                stateMachine.collider.isTrigger = true;
                stateMachine.animator.SetBool("Flying", stateMachine.jumpInCount < 1 ? true : false);
                if (stateMachine.jumpInCount < 1)
                    stateMachine.jumpInCount++;
            }
            // 위를 체크하고 싶은데...
        }

        public void NewPlatformCheck()
        {
            // 0 이상일 때는 체크하지 않.기.
            // 왜냐면 올라가고 있기 때문이지
            if(stateMachine.physics.velocity.y < 0)
            {
                // rect와 비교하여 해당 위치보다 아래 있으면 체크하지 않기.
                // 그럼 경우의 수는 2가지
                // 바닥을 뚫었는가? 에 대한 체크
                if(AABBCheck())
                {
                    // 위로 올라가야함
                    Vector3 velocity = stateMachine.physics.velocity;
                    velocity.y = 0;
                    transform.position = new Vector3(transform.position.x, underPlatform.rect.y, transform.position.z);
                    stateMachine.physics.velocity = velocity; 
                }
                else
                {
                    // 바닥을 뚫지 않음.
                }
            }
        }

        private bool AABBCheck()
        {
            if (transform.position.x < underPlatform.rect.x && transform.position.x > underPlatform.rect.width &&
                transform.position.y < underPlatform.rect.y && transform.position.y > underPlatform.rect.height)
            {
                Debug.Log("Hello");
                return true;
            }
            Debug.Log("Bye");
            return false;
        }

        public void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            _gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 1.5f);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        }

        public void handleJump()
        {
            if(stateMachine.jumpInCount > 0 && stateMachine.isJumpping )
            {
                stateMachine.isJumpping = false;
                stateMachine.physics.velocity = Vector3.up * initialJumpVelocity;
            }
            if(stateMachine.physics.velocity.y > 0 && !stateMachine.isKnockback)
            {
                stateMachine.physics.velocity += Vector3.up * _gravity * Time.deltaTime;
            }
        }

        public void DeadLineCheck()
        {
            if(!stateMachine.isDead && transform.position.y < DeadLine)
            {
                stateMachine.isDead = true;
                AliveDelay().Forget();
            }
        }

        private async UniTaskVoid AliveDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(respawnTime));
            stateMachine.ResetVelocity();
            transform.position = underPlatform.transform.position + AliveOffset;
            stateMachine.isDead = false;
            stateMachine.damageGage = 0;
        }

        public void SetUnderPlatform()
        {
            underPlatform = GameObject.Find("Main_Floor (1)").GetComponent<Platform>();
        }


        #endregion





    }

}