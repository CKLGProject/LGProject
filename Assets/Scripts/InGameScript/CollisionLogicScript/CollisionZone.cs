using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionZone : MonoBehaviour
{
    public Rect DeadSpace = new Rect();
    public Vector3 DeadLineBoxScale = Vector3.zero;
    public GameObject prefab;
    public GameObject TargetGroup;
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public abstract bool TriggerSpace(Transform plableTransform);

}
