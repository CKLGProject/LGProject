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

        public float Speed;


        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            

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

        public override void ShootProjectile(int AtkCount, Vector3 velocity)
        {
            //base.ShootProjectile();
            Vector3 Velocity = Vector3.zero;
            switch (AtkCount)
            {
                case 1:
                    // 일당 생성으로 해보자
                    BulletPrefab1.gameObject.SetActive(true);
                    BulletPrefab1.GetComponent<ParticleSystem>().Play();
                    Velocity = Vector3.up * 0.5f;
                    BulletPrefab1.transform.position = Velocity + transform.position + (velocity);

                    velocity *= velocity.x;
                    BulletPrefab1.MoveSet(Speed, velocity, transform.position, Vector3.zero, UpGage);
                    
                    break;
                case 2:
                    // 일당 생성으로 해보자
                    BulletPrefab2.gameObject.SetActive(true);
                    BulletPrefab2.GetComponent<ParticleSystem>().Play();
                    Velocity = Vector3.up * 0.5f;
                    BulletPrefab2.transform.position = Velocity + transform.position + (velocity );

                    velocity *= velocity.x;
                    BulletPrefab2.MoveSet(Speed, velocity, transform.position, Vector3.zero, UpGage);
                    break;
                case 3:
                    // 일당 생성으로 해보자
                    BulletPrefab3.gameObject.SetActive(true);
                    BulletPrefab3.GetComponent<ParticleSystem>().Play();
                    Velocity = Vector3.up * 0.5f;
                    //V.x -= 1.25f;
                    //if (velocity.x > 0)
                    BulletPrefab3.transform.position = Velocity + transform.position + (velocity);
                    //else
                    //    BulletPrefab1.transform.position = V + transform.position + (velocity * -1.25f);

                    velocity *= velocity.x;
                    Vector3 KnockbackVelocity = (transform.forward * 1.5f + transform.up * 3) * 1.5f; 
                    BulletPrefab3.MoveSet(Speed, velocity, transform.position , KnockbackVelocity, UpGage);
                    break;
                default:
                    break;
            }

        }
    }
}