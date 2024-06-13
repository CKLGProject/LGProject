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
        private Vector3 _direction;
        private Vector3 _originPos;
        [SerializeField] private GameObject _hitEffect;

        public override bool TriggerSpace(Transform plableTransform)
        {
            throw new System.NotImplementedException();
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
            if(Vector3.Distance(transform.position, _originPos) > 3.5f)
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