using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject.CollisionZone
{
    public abstract class CollisionZone : MonoBehaviour
    {
        public Rect RectSpace = new Rect();
        public Vector3 CollisionLineBoxScale = Vector3.zero;
        public Vector3 Offset = Vector3.zero;

        public Color CollisionColor;
        //public GameObject prefab;
        //public GameObject TargetGroup;
        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
        private void OnDrawGizmos()
        {
            Gizmos.color = CollisionColor;
            //Debug.Log($"{transform.forward}");
            Vector3 OffsetPreset = new Vector3(Offset.x * transform.right.x, Offset.y, Offset.z);
            Gizmos.DrawWireCube(transform.position + OffsetPreset, CollisionLineBoxScale);
        }
        //private void Start()
        //{
        //    foreach (var item in GetComponents(typeof(CollisionZone)))
        //    {
        //        Debug.Log("A");
        //    }
        //}

        public void InitSize()
        {
            RectSpace.Set((transform.position.x + CollisionLineBoxScale.x + (Offset.x * transform.right.z)) * 0.5f, (transform.position.y + CollisionLineBoxScale.y + Offset.y) * .5f, (transform.position.x - CollisionLineBoxScale.x) * .5f, (transform.position.y - CollisionLineBoxScale.y) * .5f);
            // 여기서는 TargetGroup을 사용하지 않는다.
            //TargetGroup = GameObject.Find("Target Group (1)");
        }

        public abstract bool TriggerSpace(Transform plableTransform);

        public abstract bool TriggerSpace(Vector3 playableVector);

    }
}