using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.CollisionZone
{
    public class Platform : CollisionZone
    {
        public Vector3 center;
        public Vector3 offset;
        public List<GameObject> objList;
        public GameObject prefab;
        public Rect rect = new Rect();

        public override bool TriggerSpace(Transform plableTransform)
        {
            if (plableTransform.position.x < rect.x && plableTransform.position.x > rect.width &&
                plableTransform.position.y < rect.y && plableTransform.position.y > rect.height)
            {
                return true;
            }

            return false;
        }

        public override bool TriggerSpace(Vector3 playableVector)
        {
            if (playableVector.x < rect.x && playableVector.x > rect.width &&
                playableVector.y < rect.y && playableVector.y > rect.height)
            {
                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Collider cols = GetComponent<BoxCollider>();
            offset = new Vector3(cols.bounds.size.x, cols.bounds.size.y, cols.bounds.size.z);
            center = cols.bounds.center;
            Gizmos.DrawWireCube(center, offset);

        }
        private void Start()
        {
            //// 네 개의 정점에 오브젝트 생성시켜주기
            //objList.Add(Instantiate(prefab, new Vector3((center.x + offset.x) * 0.5f, (center.y + offset.y) * 0.5f, -9.5f), Quaternion.identity));
            //objList.Add(Instantiate(prefab, new Vector3((center.x - offset.x) * 0.5f, (center.y + offset.y) * 0.5f, -9.5f), Quaternion.identity));
            //objList.Add(Instantiate(prefab, new Vector3((center.x + offset.x) * 0.5f, (center.y - offset.y) * 0.5f, -9.5f), Quaternion.identity));
            //objList.Add(Instantiate(prefab, new Vector3((center.x - offset.x) * 0.5f, (center.y - offset.y) * 0.5f, -9.5f), Quaternion.identity));

            rect.Set((center.x + offset.x * 0.5f) , (center.y + offset.y * 0.5f) , (center.x - offset.x * 0.5f) , (center.y - offset.y * 0.5f) );

        }
    }
}