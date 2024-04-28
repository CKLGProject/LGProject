using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LGProject
{
    public class MovePointManager : MonoBehaviour
    {
        private static MovePointManager instance = null;
        public static MovePointManager Instance
        {
            get
            {
                if(instance == null)
                {
                    GameObject obj = Instantiate(new GameObject());
                    instance = obj.AddComponent<MovePointManager>();
                }    
                return instance;
            }
        }
        ///////////////////////////////////////////////////
        ////                Fields                  ///////
        ///////////////////////////////////////////////////
        [SerializeField,]
        private Transform[] Points = null; // �곻�� �������� ���;���.
            

        public void GetPointsAtChildren()
        {
            Points = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                // ������ ����
                Points[i] = transform.GetChild(i);
            }
        }

        public Transform GetPoint()
        {
            int rand = Random.Range(0, Points.Length);
            return Points[rand];
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                // �ϴ� �Ʒ��� �ΰ��� �̱����� ����.
                Destroy(this.gameObject);
            }
            // ������Ʈ �Ʒ��� �ִ� ����� ���� ������.
            GetPointsAtChildren();
        }
    }
}