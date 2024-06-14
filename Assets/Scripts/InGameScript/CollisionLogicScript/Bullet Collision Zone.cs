using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace LGProject.CollisionZone
{
    public class BulletCollisionZone : CollisionZone
    {
        private float _speed;
        [SerializeField] private Vector3 _direction;
        [SerializeField] private Vector3 _originPos;
        [SerializeField] private GameObject _hitEffect;
        [SerializeField] private float _range;
        [SerializeField] private BoxCollider _boxCollider;
        [SerializeField] private int _damageCount;
        [SerializeField] public Vector3 KnockbackVelocity;

        // 충돌 판정을 넣고 싶은데... 상대방을 체크할 수 있는 방법이 없을까?

        public override bool TriggerSpace(Transform plableTransform)
        {
            throw new NotImplementedException();
        }
        public override bool TriggerSpace(Vector3 playableVector)
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
        }
        private void Start()
        {
            InitSize();
        }

        public void MoveSet(float speed, Vector3 direction, float timer, Vector3 velocity)
        {
            _speed = speed;
            _direction = direction;
            Play(timer).Forget();
        }
        public void MoveSet(float speed, Vector3 direction, Vector3 originPos, Vector3 velocity)
        {
            _speed = speed;
            _direction = direction;
            _originPos = originPos;
            _boxCollider.center = new Vector3(_boxCollider.center.x * transform.right.x, _boxCollider.center.y, _boxCollider.center.z);
            KnockbackVelocity = velocity;
        }

        private void Update()
        {
            if(Vector3.Distance(transform.position, _originPos) > _range)
            {
                gameObject.SetActive(false);
                return;
            }
            // 이동... 이동... 이동...
            transform.Translate(_direction * _speed * Time.deltaTime);
        }

        private async UniTaskVoid Play(float time)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time));
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log($"other {other.gameObject.layer} / {other.transform.name}");
            //if (other.gameObject.layer == 3)
            //{
            //    //Debug.Log("Hello1");
            //    gameObject.SetActive(false);
            //}
            //Debug.Log("World1");
        }


        private void OnCollisionEnter(Collision collision)
        {
            // 플레이어일 경우 결과 값을 반환함.
            if (collision.gameObject.layer == 1 << 3)
            {
                gameObject.SetActive(false);

            }

        }

    }
}