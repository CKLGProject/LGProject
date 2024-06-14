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

        public override bool TriggerSpace(Transform plableTransform)
        {
            throw new System.NotImplementedException();
        }
        public override bool TriggerSpace(Vector3 playableVector)
        {
            throw new NotImplementedException();
        }

        public void MoveSet(float speed, Vector3 direction, float timer)
        {
            _speed = speed;
            _direction = direction;
            Play(timer).Forget();
        }
        public void MoveSet(float speed, Vector3 direction, Vector3 originPos)
        {
            _speed = speed;
            _direction = direction;
            _originPos = originPos;
        }

        private void Update()
        {
            Debug.Log(Vector3.Distance(transform.position, _originPos));
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

    }
}