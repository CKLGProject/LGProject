using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerState  // 
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

        // ĳ�� 
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


        // ���� ť -> �Է����� �� ���� Ÿ�̸� �ʱ�ȭ 1�ʾ� �ȿ� �ȴ����� �ʱ�ȭ?
        private Queue<E_KEYTYPE> comboQueue;


        public bool isGrounded;
        public bool isGuard;
        public bool isJumpGuard;
        public bool isHit;
        public bool isAttack;
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
                Debug.LogWarning("Player Component�� Input System�� ���õ� Component�� �������� �ʽ��ϴ�.\n������ Playerable Character�� �������� ���� ���� �ֽ��ϴ�.");
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

        public bool CheckEnemy()
        {
            Vector3 downPos = transform.position + (Vector3.down * 0.9f);
            Vector3 upPos = transform.position + (Vector3.up * 0.9f);
            Vector3 direction = Vector3.right * (moveAction.ReadValue<float>() >= 0 ? +1.5f : -1.5f);
            Debug.Log($"Direction = {direction}");
            // 0�� ��� ���������� ������ �ؾ��ϴ��� üũ�غ���.

            Ray downRay = new Ray(downPos, direction);
            Ray upRay = new Ray(upPos, direction);
            if (Physics.Raycast(downRay, 1f))
            {
                //Debug.Log("down");
                return true;
            }
            
            if(Physics.Raycast(upRay, 1f))
            {
                //Debug.Log("up");
                return true;
            }

            return false;
            //if()
        }

        /// <summary>
        /// x,z 0���� �ʱ�ȭ
        /// </summary>
        public void StandingVelocity()
        {
            Vector3 temp = Vector3.zero;
            temp.y = physics.velocity.y;
            physics.velocity = temp;
        }


        /// <summary>
        /// y�� 0���� �ʱ�ȭ
        /// </summary>
        public void JumpVelocity()
        {
            Vector3 temp = physics.velocity;
            temp.y = 0;
            physics.velocity = temp;
        }

        public void HitDamaged(Vector3 velocity)
        {
            // ��ݿ� ���� ������ ����
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