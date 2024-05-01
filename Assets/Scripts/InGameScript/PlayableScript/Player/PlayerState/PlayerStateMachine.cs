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


        // 스택 큐 -> 입력있을 때 마다 타이머 초기화 1초안 안에 안누르면 초기화?
        private Queue<E_KEYTYPE> comboQueue;


        public bool isGrounded;
        public bool isGuard;
        public bool isJumpGuard;
        public bool isHit;
        public bool isDown;

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

        public static PlayerStateMachine CreateStateMachine(GameObject obj)
        {
            PlayerStateMachine psm = new PlayerStateMachine();
            psm.transform = obj.transform;
            psm.playable = obj.GetComponent<Playable>();
            psm.animator = obj.GetComponent<Animator>();
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
            // 충격에 의한 물리를 제공
            physics.velocity = velocity;

            isHit = true;
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