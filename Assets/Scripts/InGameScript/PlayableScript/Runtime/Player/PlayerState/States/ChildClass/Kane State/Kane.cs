using Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.PlayerState
{
    public class Kane : Player
    {
        public CollisionZone.BulletCollisionZone BulletPrefab1;
        public CollisionZone.BulletCollisionZone BulletPrefab2;
        public CollisionZone.BulletCollisionZone BulletPrefab3;
        public CollisionZone.BulletCollisionZone JumpAttackBulletPrefab;

        public float Speed;


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            BulletPrefab1.ParentPlayerSet(transform);
            BulletPrefab2.ParentPlayerSet(transform);
            BulletPrefab3.ParentPlayerSet(transform);
            JumpAttackBulletPrefab.ParentPlayerSet(transform);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public void UpGage()
        {
            SetUltimateGage(UltimateGage + 10);
        }

        public override void ShootProjectile(int attackCount, Vector3 velocity)
        {
            //base.ShootProjectile();

            // 총을 쏠 때 쿵 FX 출력
            StateMachine.VocaFX.PlayVoca(EVocaType.Tang);

            Vector3 startPosition;
            Vector3 knockbackVelocity;
            switch (attackCount)
            {
                case 1:
                    // 일당 생성으로 해보자
                    BulletPrefab1.gameObject.SetActive(true);
                    BulletPrefab1.GetComponent<ParticleSystem>().Play();
                    startPosition = Vector3.up * 0.5f;
                    BulletPrefab1.transform.position = startPosition + transform.position + velocity;
                    //velocity.x *= 5f;
                    velocity *= velocity.x;
                    BulletPrefab1.MoveSet(Speed, velocity, transform.position, Vector3.zero, UpGage);

                    break;
                case 2:
                    // 일당 생성으로 해보자
                    BulletPrefab2.gameObject.SetActive(true);
                    BulletPrefab2.GetComponent<ParticleSystem>().Play();
                    startPosition = Vector3.up * 0.5f;
                    BulletPrefab2.transform.position = startPosition + transform.position + velocity;

                    velocity *= velocity.x;
                    BulletPrefab2.MoveSet(Speed, velocity, transform.position, Vector3.zero, UpGage);
                    break;
                case 3:
                    // 일당 생성으로 해보자
                    BulletPrefab3.gameObject.SetActive(true);
                    BulletPrefab3.GetComponent<ParticleSystem>().Play();
                    startPosition = Vector3.up * 0.5f;

                    BulletPrefab3.transform.position = startPosition + transform.position + velocity;

                    velocity *= velocity.x;
                    knockbackVelocity = (transform.forward * 1.5f + transform.up * 3) * 1.5f;
                    BulletPrefab3.MoveSet(Speed, velocity, transform.position, knockbackVelocity, UpGage);
                    break;

                case 4:

                    JumpAttackBulletPrefab.gameObject.SetActive(true);
                    JumpAttackBulletPrefab.GetComponent<ParticleSystem>().Play();
                    //StartPoint = Vector3.up * 0.5f ;
                    startPosition = transform.forward * -1f + transform.up * 1f;

                    JumpAttackBulletPrefab.transform.position = startPosition + transform.position + (velocity);
                    JumpAttackBulletPrefab.transform.rotation =
                        Quaternion.Euler(0, 0, transform.forward.x < 0 ? -135 : -45);
                    velocity *= velocity.x;
                    knockbackVelocity = (transform.forward * 1.5f + transform.up * 3) * 1.5f;
                    JumpAttackBulletPrefab.MoveSet(Speed, velocity, transform.position, knockbackVelocity, UpGage);
                    break;
            }
        }
    }
}