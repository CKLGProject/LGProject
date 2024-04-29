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
        private Transform[] Points = null; // ¾ê³»¸¦ ·£´ýÀ¸·Î »©¿Í¾ßÇÔ.
            

        public void GetPointsAtChildren()
        {
            Points = new Transform[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                // ½ºÅåÀ» ÀúÀå
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
                // ÇÏ´Ã ¾Æ·¡¿¡ µÎ°³ÀÇ ½Ì±ÛÅæÀº ¾ø´Ù.
                Destroy(this.gameObject);
            }
            // ÄÄÆ÷³ÍÆ® ¾Æ·¡¿¡ ÀÖ´Â ³ðµéÀ» ÀüºÎ °¡Á®¿È.
            GetPointsAtChildren();
        }
    }
}