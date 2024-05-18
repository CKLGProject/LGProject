using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LGProject.PlayerState  // 
{
    public class PlayerStateMachine
    {
        public enum E_KEYTYPE
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            JUMP,
            ATTACk,
        }

        // 캐싱
        public Transform transform;
        public Playable playable;
        public Animator animator;
        public Rigidbody physics;
        public PlayerInput playerInput;
        public Collider collider;

        public InputAction moveAction;
        public InputAction attackAction;
        public InputAction jumpAction;
        public InputAction downAction;
        public InputAction guardAction;

        public IdleState idleState;
        public MoveState moveState;
        public JumpState jumpState;
        public AttackState attackState;
        public JumpAttackState jumpAttackState;
        public DashAttackState dashAttackState;
        public HitState hitState;
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
        public bool IsUltimate;

        public bool IsDashAttack = false;
        public bool IsJumpAttack = false;

        //public GameObject GuardEffect;

        public float DamageGage = 0;
        public float UltimateGage = 0;

        #region Action_Properties


        #endregion

        public int JumpInCount = 0;
        public int AttackCount = 0;

        public State CurrentState;
        public Transform hitPlayer;

        private Dictionary<string, float> animClipsInfo = new Dictionary<string, float>();

        public void SetAnimPlayTime(string clipName, float time)
        {
            animClipsInfo.Add(clipName, time);
        }

        public float GetAnimPlayTime(string clipName)
        {
            float value = 0;
            animClipsInfo.TryGetValue(clipName, out value);
            return value;
        }

        public static PlayerStateMachine CreateStateMachine(GameObject obj)
        {
            PlayerStateMachine psm = new PlayerStateMachine();
            psm.transform = obj.transform;
            psm.playable = obj.GetComponent<Playable>();
            //psm.animator = obj.GetComponent<Animator>();
            psm.animator = psm.playable.Animator;
            psm.physics = obj.GetComponent<Rigidbody>();
            psm.playerInput = obj.GetComponent<PlayerInput>();
            psm.collider = obj.GetComponent<Collider>();

            psm.IsGrounded = true;
            psm.IsGuard = false;
            try
            {
                psm.moveAction = psm.playerInput.actions["Move"];
                psm.attackAction = psm.playerInput.actions["Attack"];
                psm.jumpAction = psm.playerInput.actions["Jump"];
                psm.downAction = psm.playerInput.actions["Down"];
                psm.guardAction = psm.playerInput.actions["Guard"];

                psm.idleState = new IdleState(psm);
                psm.moveState = new MoveState(psm, ref psm.playable.DashSpeed, psm.playable.MaximumSpeed);
                psm.jumpState = new JumpState(psm, ref psm.playable.JumpScale, psm.playable.MaximumJumpCount, psm.playable.jumpCurve);

                psm.attackState = new AttackState(psm, ref psm.playable.FirstAttackJudgeDelay, ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackJudgeDelay, ref psm.playable.SecondAttackDelay, ref psm.playable.ThirdAttackJudgeDelay, ref psm.playable.ThirdAttackDelay);
                psm.ultimateState = new UltimateState(psm);

                psm.jumpAttackState = new JumpAttackState(psm, psm.playable.MaximumSpeed);
                psm.dashAttackState = new DashAttackState(psm, ref psm.playable.DashAttackDelay);

                psm.hitState = new HitState(psm, ref psm.playable.HitDelay);
                psm.guardState = new GuardState(psm);

                psm.ultimateState = new HitUltimateState(psm);
                psm.downState = new DownState(psm, ref psm.playable.DownWaitDelay);
                psm.wakeUpState = new WakeUpState(psm, ref psm.playable.WakeUpDelay);
                psm.landingState = new LandingState(psm);

                psm.Initalize(psm.idleState);

            }
            catch
            {
                Debug.LogWarning("Player Component에 Input System과 관련된 Component가 존재하지 않습니다.\n때문에 Playerable Character가 움직이지 않을 수도 있습니다.");
            }
            return psm;
        }

        // 캐릭터 별로 State맞추기
        public void SetUltimateState()
        {

        }

        public void Initalize(State startingState)
        {
            CurrentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(State nextState)
        {
            CurrentState.Exit();
            CurrentState = nextState;
            CurrentState.Enter();
        }

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
            
            if(Physics.Raycast(upRay, out hit, .5f, 1 << 3))
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
    }

        public void ResetVelocity()
        {
            physics.velocity = Vector3.zero;
        }

        public void HitDamaged(Vector3 velocity)
        {
            // 누어 있는 상태에선 데미지를 입지 않는다.
            if (IsDown && IsUltimate)
                return;
            if (!IsGuard)
            {
                DamageGage += 8.5f;
                SetDamageGageOnText();
                IsNormalAttack = false;
                animator.SetTrigger("Hit");
                // 충격에 의한 물리 공식
                velocity *= Mathf.Pow(2, (DamageGage * 0.01f));
                physics.velocity = velocity;
                if (velocity != Vector3.zero)
                {
                    //GameObject.Instantiate(new GameObject(), transform.position + velocity, Quaternion.identity);
                    animator.SetTrigger("Knockback");
                    IsKnockback = true;
                }
                //playable.effectManager.Play(EffectManager.EFFECT.Hit).Forget();
            }
            else
            {
                physics.velocity = new Vector3(transform.forward.x * -2f, 0, 0);
            }
            IsDamaged = true;
        }

        public void SetDamageGageOnText()
        {
            // Gage 상승
            int a = (int)(DamageGage);
            //= 0;
            int b = (int)((DamageGage - a) * 10);

            if (playable.DamageGageInt != null)
                playable.DamageGageInt.text = $"<rotate=\"0\">{a}.</rotate>";
            if (playable.DamageGageDecimal != null)
                playable.DamageGageDecimal.text = $"<rotate=\"0\">{b}%</rotate>";
        }

        public void ResetAnimParameters()
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("DashAttack");
            animator.ResetTrigger("Hit");
            //animator.ResetTrigger("Guard");
            animator.ResetTrigger("Jump1");
            animator.ResetTrigger("Jump2");
            animator.ResetTrigger("Knockback");
            animator.ResetTrigger("Landing");
            animator.ResetTrigger("WakeUp");
        }

        public void UltimateGageisFull()
        {
            if(UltimateGage >= 100)
            {
                playable.ShowUltimateEffect();
            }
        }


        #region ComboMethods


        // combo System
        public bool InputCombo(E_KEYTYPE keyType)
        {
            if(CurrentState.GetType() == typeof(JumpState) && true) 
            {
                return false;
            }
            else if(CurrentState.GetType() == typeof(AttackState))
            {
                return true;
            }
            //comboQueue
            //comboQueue.Enqueue(keyType);
            return false;
        }



        public void ComboTimer()
        {
            //if(comboQueue.Count > 0)
            //{

            //}
        }
        #endregion
    }

}