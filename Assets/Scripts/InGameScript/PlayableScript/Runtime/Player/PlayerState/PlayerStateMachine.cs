using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using Data;
using FMODPlus;
using Object = UnityEngine.Object;
using FMODUnity;

namespace LGProject.PlayerState
{
    public enum DAMAGE_TYPE
    {
        NormalAttack,
        DashAttack,
        JumpAttack,
        Guard,
        Chasing,
        Movement,
    }

    public class PlayerStateMachine
    {
        // 캐싱
        public Transform transform;
        public Playable playable;
        public Animator animator;
        public FMODAudioSource AudioSource;
        public LocalKeyList AudioList;
        public Rigidbody physics;
        public Collider collider;
        public BattleModel battleModel;
        public VocaFX VocaFX;

        public InputAction moveAction;
        public InputAction attackAction;
        public InputAction jumpAction;
        public InputAction downAction;
        public InputAction guardAction;

        public IdleState idleState;
        public MoveState moveState;
        public JumpState jumpState;
        public FlightState flightState;
        public AttackState attackState;
        public JumpAttackState jumpAttackState;
        public DashAttackState dashAttackState;
        public HitState hitState;
        public KnockbackState knockbackState;
        public GuardState guardState;
        public DownState downState;
        public WakeUpState wakeUpState;
        public UltimateState ultimateState;
        public LandingState landingState;

        // 스택 큐 -> 입력있을 때 마다 타이머 초기화 1초안 안에 안누르면 초기화?
        //private Queue<E_KEYTYPE> comboQueue;

        public bool IsGrounded;
        public bool IsGuard;
        public bool IsJumpGuard;
        public bool IsDamaged;
        public bool IsDown;
        public bool IsKnockback;
        public bool IsJumpping;
        public bool IsDead;
        public bool IsNormalAttack;
        public bool IsUseUltimate;
        public bool IsUltimate;
        public bool IsSuperArmor;

        public bool IsDashAttack = false;
        public bool IsJumpAttack = false;

        #region Action_Properties

        #endregion

        public int JumpInCount = 0;
        public int AttackCount = 0;

        // 이건 어떻게 깎이게 할 것인가?
        public float GuardGage = 100;

        public State CurrentState;
        public Transform HitPlayer;

        public ECharacterType CharacterType;

        public bool IsPet;

        private Dictionary<string, float> _animationClipsInfo = new Dictionary<string, float>();

        // Constant
        private static readonly int Hit = Animator.StringToHash("Hit");
        private static readonly int Knockback = Animator.StringToHash("Knockback");
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int DashAttack = Animator.StringToHash("DashAttack");
        private static readonly int Jump1 = Animator.StringToHash("Jump1");
        private static readonly int Jump2 = Animator.StringToHash("Jump2");
        private static readonly int Landing = Animator.StringToHash("Landing");
        private static readonly int WakeUp = Animator.StringToHash("WakeUp");
        private static readonly int GuardEnd = Animator.StringToHash("GuardEnd");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="time"></param>
        public void SetAnimationPlayTime(string clipName, float time)
        {
            //_animationClipsInfo.Add(clipName, time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clipName"></param>
        /// <returns></returns>
        public float GetAnimationPlayTime(string clipName)
        {
            _animationClipsInfo.TryGetValue(clipName, out float value);
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static PlayerStateMachine CreateStateMachine(GameObject obj)
        {
            PlayerStateMachine psm = new PlayerStateMachine();
            psm.transform = obj.transform;
            psm.playable = obj.GetComponent<Playable>();
            psm.AudioSource = psm.playable.AudioSourceProperty;
            psm.animator = psm.playable.Animator;
            psm.physics = obj.GetComponent<Rigidbody>();
            psm.physics.isKinematic = false;
            psm.collider = obj.GetComponent<Collider>();
            psm.collider.isTrigger = false;
            psm.CharacterType = psm.playable.CharacterType;
            psm.IsGrounded = true;
            psm.IsGuard = false;

            if (psm.playable.ActorType == ActorType.User)
                psm.IsPet = GameObject.Find("User Pet") != null ? true : false;
            else
                psm.IsPet = GameObject.Find("AI Pet") != null ? true : false ;

            try
            {
                psm.moveAction = InputSystem.actions["Move"];
                psm.attackAction = InputSystem.actions["Attack"];
                psm.jumpAction = InputSystem.actions["Jump"];
                psm.downAction = InputSystem.actions["Down"];
                psm.guardAction = InputSystem.actions["Guard"];

                psm.idleState = new IdleState(psm);
                psm.moveState = new MoveState(psm, ref psm.playable.DashSpeed, psm.playable.MaximumSpeed);
                psm.jumpState = new JumpState(psm, ref psm.playable.JumpScale, psm.playable.MaximumJumpCount,
                    psm.playable.jumpCurve);
                psm.flightState = new FlightState(psm);
                ApplyAttackState(psm.CharacterType, psm);

                ApplyJumpAttackState(psm.CharacterType, psm);

                psm.hitState = new HitState(psm, ref psm.playable.HitDelay);
                psm.guardState = new GuardState(psm);
                psm.knockbackState = new KnockbackState(psm);

                psm.downState = new DownState(psm, ref psm.playable.DownWaitDelay);
                psm.wakeUpState = new WakeUpState(psm, ref psm.playable.WakeUpDelay);
                psm.landingState = new LandingState(psm);

                

                psm.Initalize(psm.idleState);
                //SetUltimateState();
            }
            catch
            {
#if UNITY_EDITOR
                Debug.LogWarning(
                    "Player Component에 Input System과 관련된 Component가 존재하지 않습니다.\n때문에 Playerable Character가 움직이지 않을 수도 있습니다.");
#endif
            }

            return psm;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterType"></param>
        /// <param name="psm"></param>
        private static void ApplyAttackState(ECharacterType characterType, PlayerStateMachine psm)
        {
            switch (characterType)
            {
                case ECharacterType.None:
                    psm.attackState = new AttackState(psm, ref psm.playable.FirstAttackJudgeDelay,
                        ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackJudgeDelay,
                        ref psm.playable.SecondAttackDelay, ref psm.playable.ThirdAttackJudgeDelay,
                        ref psm.playable.ThirdAttackDelay);
                    break;
                case ECharacterType.Hit:
                    psm.attackState = new HitAttackState(psm, ref psm.playable.FirstAttackJudgeDelay,
                        ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackJudgeDelay,
                        ref psm.playable.SecondAttackDelay, ref psm.playable.ThirdAttackJudgeDelay,
                        ref psm.playable.ThirdAttackDelay);
                    break;
                case ECharacterType.Frost:
                    psm.attackState = new FrostAttackState(psm, ref psm.playable.FirstAttackJudgeDelay,
                        ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackJudgeDelay,
                        ref psm.playable.SecondAttackDelay, ref psm.playable.ThirdAttackJudgeDelay,
                        ref psm.playable.ThirdAttackDelay);
                    break;
                case ECharacterType.Kane:
                    psm.attackState = new KaneAttackState(psm, ref psm.playable.FirstAttackJudgeDelay,
                        ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackJudgeDelay,
                        ref psm.playable.SecondAttackDelay, ref psm.playable.ThirdAttackJudgeDelay,
                        ref psm.playable.ThirdAttackDelay);
                    break;
                case ECharacterType.Storm:
                    break;
                case ECharacterType.E:
                    break;
                default:
                    break;
            }
        }

        public static void ApplyJumpAttackState(ECharacterType characterType, PlayerStateMachine psm)
        {
            switch (characterType)
            {
                case ECharacterType.None:
                    break;
                case ECharacterType.Hit:
                    psm.jumpAttackState = new JumpAttackState(psm, psm.playable.MaximumSpeed, "Hit_Attack");
                    psm.dashAttackState = new DashAttackState(psm, ref psm.playable.DashAttackDelay, "Hit_Attack");
                    break;
                case ECharacterType.Frost:
                    psm.jumpAttackState = new JumpAttackState(psm, psm.playable.MaximumSpeed, "Frost_Attack");
                    psm.dashAttackState = new DashAttackState(psm, ref psm.playable.DashAttackDelay, "Frost_Attack");
                    break;
                case ECharacterType.Kane:
                    psm.jumpAttackState = new KaneJumpAttackState(psm, psm.playable.MaximumSpeed, "Kane_Attack");
                    psm.dashAttackState = new DashAttackState(psm, ref psm.playable.DashAttackDelay, "Hit_Attack");
                    break;
                case ECharacterType.Storm:
                    break;
                case ECharacterType.E:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterType"></param>
        /// <param name="speed"></param>
        public void AnimationSpeed(ECharacterType characterType, float speed)
        {
            switch (characterType)
            {
                case ECharacterType.None:
                    break;
                case ECharacterType.Hit:
                    animator.speed = speed;
                    break;
                case ECharacterType.Frost:
                    animator.speed = speed;
                    break;
                case ECharacterType.Kane:
                    animator.speed = speed;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckFlight()
        {
            if (CurrentState.GetType() == typeof(JumpState) ||
                CurrentState.GetType() == typeof(FlightState) ||
                CurrentState.GetType() == typeof(JumpAttackState) ||
                CurrentState.GetType() == typeof(KaneJumpAttackState) ||
                CurrentState.GetType() == typeof(KnockbackState) ||
                CurrentState.GetType() == typeof(GuardState))
                return true;
            return false;
        }

        public bool CheckFlightAI()
        {
            if (!IsGrounded && !IsDead && JumpInCount > 0 && !IsDamaged)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        public async UniTaskVoid ResetAnimSpeed(float time = 0f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
            animator.speed = 1f;
        }

        // 캐릭터 별로 State맞추기
        public void SetUltimateState(ECharacterType charcterType)
        {
            switch (charcterType)
            {
                case ECharacterType.None:

                    break;
                case ECharacterType.Hit:
                    ultimateState = new HitUltimateState(this, playable.UltimateDelay);
                    break;
                case ECharacterType.Frost:
                    ultimateState = new FrostUltimateState(this, playable.UltimateDelay);
                    break;
                case ECharacterType.Kane:
                    ultimateState = new KaneUltimateState(this, playable.UltimateDelay);
                    break;
                default:
                    break;
            }
        }

        public bool CheckPlatform(Vector3 checkPos)
        {
            return playable.CheckPointCollision(checkPos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingState"></param>
        public void Initalize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nextState"></param>
        public void ChangeState(State nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Transform CheckEnemy()
        {
            Vector3 downPos = transform.position;
            Vector3 upPos = transform.position + (Vector3.up * 0.5f);
            Vector3 direction = Vector3.right * (playable.directionX ? +1.5f : -1.5f);
            // 0일 경우 어디방향으로 공격을 해야하는지 체크해보자.

            Ray downRay = new Ray(downPos, direction);
            Ray upRay = new Ray(upPos, direction);
            RaycastHit hit;
            if (Physics.Raycast(downRay, out hit, .5f, 1 << 3))
            {
                return hit.transform;
            }

            if (Physics.Raycast(upRay, out hit, .5f, 1 << 3))
            {
                return hit.transform;
            }

            return null;
        }

        /// <summary>
        /// x,z 0으로 초기화
        /// </summary>
        public void StandingVelocity()
        {
            Vector3 temp = Vector3.zero;
            temp.y = physics.velocity.y;
            physics.velocity = temp;
        }


        /// <summary>
        /// y값 0으로 초기화
        /// </summary>
        public void JumpVelocity()
        {
            Vector3 temp = physics.velocity;
            temp.y = 0;
            physics.velocity = temp;
        }

        public void Update()
        {
            playable.IsGrounded = IsGrounded;
            playable.IsGuard = IsGuard;
            playable.IsJumpGuard = IsJumpGuard;
            playable.IsDamaged = IsDamaged;
            playable.IsDown = IsDown;
            playable.IsKnockback = IsKnockback;
            playable.IsJumpping = IsJumpping;
            playable.IsDead = IsDead;
            playable.IsNormalAttack = IsNormalAttack;
            playable.JumpCount = JumpInCount;
            playable.SupperAmmor = IsSuperArmor;
            playable.IsPet = IsPet;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetVelocity()
        {
            physics.velocity = Vector3.zero;
        }

        public void JumpAttackVelocity()
        {
            physics.velocity = transform.up * 1.5f;
        }

        public void SuperAmmor()
        {
            IsSuperArmor = true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageType"></param>
        public void DataSet(DATA_TYPE damageType)
        {
            UpdateData(damageType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageType"></param>
        public void UpdateData(DATA_TYPE damageType)
        {
            try
            {
                switch (damageType)
                {
                    case DATA_TYPE.NormalAttack:
                        FileManager.Instance.SetInGameData(DATA_TYPE.NormalAttack);
                        break;
                    case DATA_TYPE.DashAttack:
                        FileManager.Instance.SetInGameData(DATA_TYPE.DashAttack);
                        break;
                    case DATA_TYPE.NormalAttackHit:
                        FileManager.Instance.SetInGameData(DATA_TYPE.NormalAttackHit);
                        break;
                    case DATA_TYPE.DashAttackHit:
                        FileManager.Instance.SetInGameData(DATA_TYPE.DashAttackHit);
                        break;
                    case DATA_TYPE.Jump:
                        FileManager.Instance.SetInGameData(DATA_TYPE.Jump);
                        break;
                    case DATA_TYPE.Chasing:
                        FileManager.Instance.SetInGameData(DATA_TYPE.Chasing);
                        break;
                    case DATA_TYPE.Movement:
                        FileManager.Instance.SetInGameData(DATA_TYPE.Movement);
                        break;
                    case DATA_TYPE.Guard:
                        FileManager.Instance.SetInGameData(DATA_TYPE.Guard);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
#if UNITY_EDITOR
                Debug.Log("Don't have File Manager");
#endif
            }
        }

        private readonly string DamagedSFXName = "Damaged";

        /// <summary>
        /// velocity     = 넉백 방향 및 날아가는 힘.
        /// nockbackDelay = 힘을 적용 받기까지의 시간.
        /// Damaged = 넉백 증가 수치.
        /// EnemyStateMachine = 피해를 준 플레이어의 정보.
        /// dataType = 피해를 받은 타입.
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="nockbackDelay"></param>
        /// <param name="EnemyStateMachine"></param>
        /// <param name="DamageGage"></param>
        /// <param name="dataType"></param>
        public void ApplyHitDamaged(Vector3 velocity, float nockbackDelay, PlayerStateMachine EnemyStateMachine,
            float DamageGage = 8.5f)
        {
            //UpdateData(dataType);
            // 누어 있는 상태에선 데미지를 입지 않는다.
            if (IsDown || IsUseUltimate || IsSuperArmor || (EnemyStateMachine != null && IsUseUltimate))
                return;
            if (IsGuard)
            {
                physics.velocity =
                    new Vector3((EnemyStateMachine.transform.position - transform.position).normalized.x * -2.5f, 0, 0);
            }

            if (!IsGuard || (EnemyStateMachine != null && EnemyStateMachine.IsUseUltimate))
            {
                PlayAudioClip(DamagedSFXName);
                physics.velocity = Vector3.zero;
                float UpperDamageGage = playable.DamageGage + (IsUltimate == true ? DamageGage * 0.5f : DamageGage);
                UpperDamageGage *= EnemyStateMachine.IsPet ? 1.1f : 1f;
                playable.SetDamageGage(UpperDamageGage);
                battleModel.SyncDamageGage(playable.ActorType, playable.DamageGage);
                IsNormalAttack = false;
                // 충격에 의한 물리 공식
                velocity *= Mathf.Pow(2, (playable.DamageGage * 0.01f));
                if (velocity.y > Vector3.zero.y)
                {
                    Debug.Log($"{velocity}");
                    SetVelocity(velocity, nockbackDelay).Forget();
                    animator.SetTrigger(Knockback);
                }
                else
                {
                    physics.velocity = velocity;
                    animator.SetTrigger(Hit);
                }

                if (EnemyStateMachine != null && EnemyStateMachine.IsUseUltimate)
                {
                    playable.effectManager.PlayOneShot(EffectManager.EFFECT.UltimateHit, Vector3.left);
                }
                else
                {
                    VocaFX.PlayVoca(EVocaType.Fuck);
                    playable.effectManager.Play(EffectManager.EFFECT.Hit).Forget();
                }

                playable.effectManager.Stop(EffectManager.EFFECT.Guard);
            }

            IsDamaged = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="nockbackDelay"></param>
        private async UniTaskVoid SetVelocity(Vector3 velocity, float nockbackDelay = 0.2f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(nockbackDelay));
            physics.velocity = velocity;
            collider.isTrigger = true;
            IsKnockback = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShootProjectile()
        {
            try
            {
                playable.ShootProjectile(IsGrounded ? AttackCount : 4, transform.forward);
            }
            catch
            {
#if UNITY_EDITOR
                Debug.Log($"{transform.name}");
#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetAnimationParameters()
        {
            animator.ResetTrigger(Idle);
            animator.ResetTrigger(DashAttack);
            animator.ResetTrigger(Hit);
            animator.ResetTrigger(Jump1);
            animator.ResetTrigger(Jump2);
            animator.ResetTrigger(Knockback);
            animator.ResetTrigger(Landing);
            animator.ResetTrigger(WakeUp);
            animator.ResetTrigger(GuardEnd);
        }

        public void UltimateGageIsFull()
        {
            if (playable.UltimateGage >= 100)
                playable.ShowUltimateEffect();
        }

        /// <summary>
        /// 오디오 클립을 이름으로 찾아 재생하는 매서드
        /// </summary>
        /// <param name="clipName"></param>
        public void PlayAudioClip(string clipName)
        {
            try
            {
                if (AudioList.TryFindClip(clipName, out EventReference clip))
                    AudioSource.PlayOneShot(clip);
                AudioSource.Play();
            }
            catch
            {
#if UNITY_EDITOR
                Debug.Log("사용할 수 있는 오디오 클립이 존재하지 않습니다.");
#endif
            }
        }

        public void BlinkPlayerMesh()
        {
            IsSuperArmor = true;
            playable.PlayerMashBlink().Forget();
        }

        /// <summary>
        /// 컴포넌트를 바인딩합니다.
        /// </summary>
        public void BindComponents()
        {
            VocaFX = playable.ActorType == ActorType.User
                ? BattleSceneSystem.Instance.UserVocaFX
                : BattleSceneSystem.Instance.AIVocaFX;

            AudioList = Object.FindAnyObjectByType<LocalKeyList>();
            battleModel = Object.FindAnyObjectByType<BattleModel>();
        }
    }
}