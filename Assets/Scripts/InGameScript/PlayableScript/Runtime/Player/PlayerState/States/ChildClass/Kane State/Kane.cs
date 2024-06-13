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

        public override void ShootProjectile(int AtkCount, Vector3 velocity)
        {
            //base.ShootProjectile();
            Debug.Log("발사");
            Vector3 V = Vector3.zero;
            switch (AtkCount)
            {
                case 1:
                    // 일당 생성으로 해보자
                    BulletPrefab1.gameObject.SetActive(true);
                    BulletPrefab1.GetComponent<ParticleSystem>().Play();
                    V = effectManager._AttackOffset1;
                    BulletPrefab1.transform.position = V + transform.position + (velocity);

                    velocity *= velocity.x;
                    BulletPrefab1.MoveSet(10f, velocity, transform.position);
                    break;
                case 2:
                    // 일당 생성으로 해보자
                    BulletPrefab2.gameObject.SetActive(true);
                    BulletPrefab2.GetComponent<ParticleSystem>().Play();
                    V = effectManager._AttackOffset1;
                    BulletPrefab2.transform.position = V + transform.position + (velocity);

                    velocity *= velocity.x;
                    BulletPrefab2.MoveSet(10f, velocity, transform.position);
                    break;
                case 3:
                    // 일당 생성으로 해보자
                    BulletPrefab3.gameObject.SetActive(true);
                    BulletPrefab3.GetComponent<ParticleSystem>().Play();
                    V = effectManager._AttackOffset1;
                    //V.x -= 1.25f;
                    //if (velocity.x > 0)
                    BulletPrefab3.transform.position = V + transform.position + (velocity);
                    //else
                    //    BulletPrefab1.transform.position = V + transform.position + (velocity * -1.25f);

                    velocity *= velocity.x;
                    BulletPrefab3.MoveSet(10f, velocity, transform.position);
                    break;
                default:
                    break;
            }

        }
    }
}