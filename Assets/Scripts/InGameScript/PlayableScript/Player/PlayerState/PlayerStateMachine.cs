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
        public LandingState landingState;

        // 스택 큐 -> 입력있을 때 마다 타이머 초기화 1초안 안에 안누르면 초기화?
        //private Queue<E_KEYTYPE> comboQueue;


        public bool isGrounded;
        public bool isGuard;
        public bool isJumpGuard;
        public bool isHit;
        public bool isDown;
        public bool isKnockback;

        public bool isNormalAttack;

        public bool isDashAttack = false;
        public bool isJumpAttack = false;

        public GameObject guardEffect;

        public int damageGage;

        #region Action_Properties


        #endregion

        public int jumpInCount = 0;
        public int attackCount = 0;

        public State currentState;
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

            psm.isGrounded = false;
            psm.isGuard = false;
            try
            {
                psm.moveAction = psm.playerInput.actions["Move"];
                psm.attackAction = psm.playerInput.actions["Attack"];
                psm.jumpAction = psm.playerInput.actions["Jump"];
                psm.downAction = psm.playerInput.actions["Down"];
                psm.guardAction = psm.playerInput.actions["Guard"];

                psm.idleState = new IdleState(psm);
                psm.moveState = new MoveState(psm, ref psm.playable.dashSpeed, psm.playable.maximumSpeed);
                psm.jumpState = new JumpState(psm, ref psm.playable.jumpScale, psm.playable.maximumJumpCount);

                psm.attackState = new AttackState(psm, ref psm.playable.FirstAttackDelay, ref psm.playable.SecondAttackDelay, ref psm.playable.ThridAttackDelay);

                psm.jumpAttackState = new JumpAttackState(psm, psm.playable.maximumSpeed);
                psm.dashAttackState = new DashAttackState(psm, ref psm.playable.dashAttackDelay);

                psm.hitState = new HitState(psm, ref psm.playable.hitDelay);
                psm.guardState = new GuardState(psm);

                psm.downState = new DownState(psm, ref psm.playable.wakeUpDelay);

                psm.landingState = new LandingState(psm);

                psm.Initalize(psm.idleState);

            }
            catch
            {
                Debug.LogWarning("Player Component에 Input System과 관련된 Component가 존재하지 않습니다.\n때문에 Playerable Character가 움직이지 않을 수도 있습니다.");
            }
            return psm;
        }

        public void Initalize(State startingState)
        {
            currentState = startingState;
            startingState.Enter();
        }

        public void ChangeState(State nextState)
        {
            currentState.Exit();

            currentState = nextState;
            currentState.Enter();
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

        public void HitDamaged(Vector3 velocity)
        {
            if (!isGuard)
            {
                animator.SetTrigger("Hit");
                // 충격에 의한 물리를 제공
                physics.velocity = velocity;
                
            }
            else
            {
                physics.velocity = new Vector3(transform.forward.x * -2f, 0, 0);
            }
            if(velocity != Vector3.zero)
            {
                animator.SetTrigger("Knockback");
                isKnockback = true;
            }
            isHit = true;
        }

        public void ResetAnimParameters()
        {
            animator.ResetTrigger("Idle");
            animator.ResetTrigger("DashAttack");
            animator.ResetTrigger("Hit");
            animator.ResetTrigger("Guard");
            animator.ResetTrigger("Jump1");
            animator.ResetTrigger("Jump2");
            animator.ResetTrigger("Knockback");
            animator.ResetTrigger("Landing");
            animator.ResetTrigger("WakeUp");
        }

        #region ComboMethods


        // combo System
        public bool InputCombo(E_KEYTYPE keyType)
        {
            if(currentState.GetType() == typeof(JumpState) && true) 
            {
                return false;
            }
            else if(currentState.GetType() == typeof(AttackState))
            {
                return true;
            }
            //comboQueue
            comboQueue.Enqueue(keyType);
            return false;
        }



        public void ComboTimer()
        {
            if(comboQueue.Count > 0)
            {

            }
        }
        #endregion
    }

}