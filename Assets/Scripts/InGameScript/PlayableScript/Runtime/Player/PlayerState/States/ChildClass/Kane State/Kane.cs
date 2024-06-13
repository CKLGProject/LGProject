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

            switch (AtkCount)
            {
                case 1:
                    // 일당 생성으로 해보자
                    BulletPrefab1.GetComponent<ParticleSystem>().Play();
                    BulletPrefab1.gameObject.SetActive(true);
                    Vector3 V = effectManager._AttackOffset1;
                    V.x -= 1.25f;
                    BulletPrefab1.transform.position = V + transform.position;
                    BulletPrefab1.MoveSet(10f, velocity, transform.position);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }

        }
    }
}