using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using Data;

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
        [field: SerializeField] public ActorType ActorType { get; private set; }
        [SerializeField] private BattleModel battleModel;

        [SerializeField] protected Vector3 velocity = Vector3.zero;

        [Tooltip("최대 점프 횟수"), SerializeField, HideInInspector]
        public int MaximumJumpCount = 2;

        [Tooltip("대쉬 최대 속도"), SerializeField, HideInInspector]
        public float MaximumSpeed = 4;

        [Tooltip("이동할 때 증가하는 속도"), SerializeField, HideInInspector]
        public float DashSpeed = 0;

        [Tooltip("한 번 점프할 때의 높이"), SerializeField, HideInInspector]
        public float JumpScale = 5;


        // 공격 관련 인스펙터 
        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴"), SerializeField, HideInInspector]
        public float FirstAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float FirstAttackJudgeDelay = 1f;

        [SerializeField, HideInInspector] public float FirstAttackMovingValue = 1f;

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴"), SerializeField, HideInInspector]
        public float SecondAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float SecondAttackJudgeDelay = 1f;

        [SerializeField, HideInInspector] public float SecondAttackMovingValue = 1f;

        [Range(0.0f, 1.0f), Tooltip("n초가 끝나면 Idle로 돌아옴"), SerializeField, HideInInspector]
        public float ThirdAttackDelay = 1f;

        [Range(0.0f, 2.0f), Tooltip("n초의 시간이 경과하면 공격 판정을 시작함."), SerializeField, HideInInspector]
        public float ThirdAttackJudgeDelay = 1f;

        [SerializeField, HideInInspector] public float ThirdAttackMovingValue = 1f;

        [Range(0.0f, 1.0f), Tooltip("공격 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float DashAttackDelay = 0;

        [Range(0.0f, 1.0f), Tooltip("피격 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float HitDelay = 0;

        [Range(0.0f, 1.0f), Tooltip("다운 후 기상 까지 걸리는 시간"), SerializeField, HideInInspector]
        public float DownWaitDelay = 0;

        [Range(0.0f, 3.0f), Tooltip("기상 후 Idle로 돌아오기 까지의 시간"), SerializeField, HideInInspector]
        public float WakeUpDelay = 0;

        [HideInInspector] public int LifePoint = 3;

        public bool movingAttack = true;

        public Image UltimateGageImage;

        public Vector3 AliveOffset;
        public float respawnTime;
        public float DeadLine;

        public float DamageGage { get; protected set; }
        public void SetDamageGage(float value) => DamageGage = value;
        
        public float UltimateGage { get; protected set; }
        public void SetUltimateGage(float value) => UltimateGage = value;

        // 공격 방향
        [HideInInspector] public bool directionX = false;
        [HideInInspector] public AnimationCurve jumpCurve;

        public Platform underPlatform;

        public EffectManager effectManager;

        public bool IsGrounded;
        public bool IsGuard;
        public bool IsJumpGuard;
        public bool IsDamaged;
        public bool IsDown;
        public bool IsKnockback;
        public bool IsJumpping;
        public bool IsDead;
        public bool IsNormalAttack;
        //public Vector3 velocity;


        float _gravity = -9.8f;
        float _groundedGravity = -0.05f;

        float initialJumpVelocity;

        //float maxJumpHeight = 1.5f;
        float maxJumpTime = 0.5f;

        protected virtual void Awake()
        {
            battleModel.InitHealth(ActorType, LifePoint);
        }

        protected PlayerStateMachine StateMachine;

        public PlayerStateMachine GetStateMachine => StateMachine;

        public Vector3 CalculateVelocity(Vector3 target, Vector3 origin, float time, float height = 1.5f)
        {
            #region Omit

            // define the distance x and y first;
            Vector3 distance = target - origin;
            Vector3 distanceXZ = distance; // x와 z의 평면이면 기본적으로 거리는 같은 벡터.
            distanceXZ.y = 0f; // y는 0으로 설정.
            //Forward = origin;
            // Create a float the represent our distance
            float Sy = distance.y; // 세로 높이의 거리를 지정.
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
            battleModel.SyncDamageGage(ActorType, DamageGage);
            if (effectManager == null)
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
            if (StateMachine.IsDown)
            {
                curTimer += Time.deltaTime;
                if (downTimer < curTimer)
                {
                    StateMachine.IsDown = false;
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
                if (!StateMachine.IsGrounded)
                {
                    effectManager.Play(EffectManager.EFFECT.Landing).Forget();
                    StateMachine.collider.isTrigger = false;
                    StateMachine.IsGrounded = true;
                    StateMachine.IsJumpGuard = false;
                    StateMachine.JumpInCount = 0;
                    StateMachine.StandingVelocity();

                    // 이거 AI랑 공용으로 사용중이라 나중에 안되게 해야함.
                    if (!StateMachine.IsKnockback && StateMachine.CurrentState != null)
                        StateMachine.ChangeState(StateMachine.landingState);

                    //stateMachine.isHit = false;
                }
            }
            else
            {
                StateMachine.IsGrounded = false;
                StateMachine.collider.isTrigger = true;
                StateMachine.animator.SetBool("Flying", StateMachine.JumpInCount < 1 ? true : false);
                if (StateMachine.JumpInCount < 1)
                    StateMachine.JumpInCount++;
            }
            // 위를 체크하고 싶은데...
        }

        private void JumpLandingCheck()
        {
            effectManager.Play(EffectManager.EFFECT.Landing).Forget();
            Vector3 velocity = StateMachine.physics.velocity;
            velocity.y = 0;
            transform.position = new Vector3(transform.position.x, underPlatform.rect.y, transform.position.z);

            StateMachine.physics.velocity = velocity;
            StateMachine.collider.isTrigger = false;
            StateMachine.IsGrounded = true;
            StateMachine.IsJumpGuard = false;
            StateMachine.JumpInCount = 0;
            StateMachine.StandingVelocity();
            if (StateMachine.CurrentState != null)
                StateMachine.ChangeState(StateMachine.landingState);
        }

        private void KncokbackLandingCheck()
        {
            effectManager.Play(EffectManager.EFFECT.Knockback).Forget();

            Vector3 velocity = StateMachine.physics.velocity;
            velocity.y = 0;
            transform.position = new Vector3(transform.position.x, underPlatform.rect.y, transform.position.z);

            StateMachine.physics.velocity = Vector3.zero;
            StateMachine.collider.isTrigger = false;
            StateMachine.IsGrounded = true;
            StateMachine.IsKnockback = false;
            StateMachine.JumpInCount = 0;
            StateMachine.StandingVelocity();
        }

        public void NewPlatformCheck()
        {
            // 0 이상일 때는 체크하지 않.기.
            // 왜냐면 올라가고 있기 때문이지
            if (StateMachine.physics.velocity.y < -0.1f &&
                (!StateMachine.IsGrounded || StateMachine.IsKnockback))
            {
                // rect와 비교하여 해당 위치보다 아래 있으면 체크하지 않기.
                // 그럼 경우의 수는 2가지
                // 바닥을 뚫었는가? 에 대한 체크
                if (AABBPlatformCheck())
                {
                    if (StateMachine.IsKnockback)
                        KncokbackLandingCheck();
                    else if (!StateMachine.IsGrounded)
                        JumpLandingCheck();
                }
            }
            // 하늘로 날아올랐다는 것을 표시
            else if (StateMachine.physics.velocity.y > 0)
            {
                // 점프를 한 상황일 때
                if (StateMachine.IsJumpping)
                {
                    StateMachine.IsGrounded = false;
                    StateMachine.collider.isTrigger = true;
                }

                // 피격당해 날아간 상태
                if (StateMachine.IsKnockback)
                {
                    StateMachine.IsGrounded = false;
                    StateMachine.collider.isTrigger = true;
                    StateMachine.IsKnockback = true;
                }
            }
        }

        private bool AABBPlatformCheck()
        {
            if (transform.position.x < underPlatform.rect.x && transform.position.x > underPlatform.rect.width &&
                transform.position.y < underPlatform.rect.y && transform.position.y > underPlatform.rect.height)
            {
                return true;
            }

            return false;
        }


        public void setupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            _gravity = (-2 * JumpScale) / Mathf.Pow(timeToApex, 1.5f);
            initialJumpVelocity = (2 * JumpScale) / timeToApex;
        }

        public void handleJump()
        {
            if (StateMachine.JumpInCount > 0 && StateMachine.IsJumpping)
            {
                StateMachine.IsJumpping = false;
            }

            if (StateMachine.physics.velocity.y > 0 && !StateMachine.IsKnockback)
            {
            }
        }

        // 점프 후 내려오는 것
        public void PlayableGravity()
        {
            if (!StateMachine.IsGrounded)
                StateMachine.physics.velocity += Vector3.up * _gravity * Time.deltaTime;
            else
                StateMachine.physics.velocity += Vector3.up * _groundedGravity * Time.deltaTime;
        }

        public void HandleJumpping()
        {
            // 키 입력으로 점프
            StateMachine.physics.velocity = Vector3.up * initialJumpVelocity;
        }

        public void DeadLineCheck()
        {
            if (!StateMachine.IsDead && transform.position.y < DeadLine)
            {
                StateMachine.IsDead = true;
                LifePoint -= 1; // 체력 감소
                battleModel.SyncHealth(ActorType, LifePoint);
                if (LifePoint > 0)
                {
                    AliveDelay().Forget();
                }
            }
        }

        private async UniTaskVoid AliveDelay()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(respawnTime));
            StateMachine.ResetVelocity();
            transform.position = Vector3.forward * -9.5f + AliveOffset;
            StateMachine.IsDead = false;
            DamageGage = 0;
            battleModel.SyncDamageGage(ActorType, DamageGage);

            StateMachine.animator.SetTrigger("Hit");
            StateMachine.IsDamaged = true;
            StateMachine.IsKnockback = true;
            StateMachine.animator.SetTrigger("Knockback");
        }

        public void SetUnderPlatform()
        {
            underPlatform = GameObject.Find("Main_Floor (1)").GetComponent<Platform>();
        }

        public void ShowUltimateEffect()
        {
            effectManager.Play(EffectManager.EFFECT.Ultimate).Forget();
        }

        #endregion
    }
}