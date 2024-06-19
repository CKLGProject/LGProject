using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{
    public Transform Follow;
    public float followSpeed = 0.15f;

    private readonly List<ParticleSystemRenderer> _particleSystemRenderers = new List<ParticleSystemRenderer>();

    private readonly Vector3 LeftDirection = Vector3.zero;
    private readonly Vector3 RightDirection = new Vector3(1, 0, 0);
    
    private void Awake()
    {
        // 파티클 시스템 렌더러를 찾아서 리스트에 추가합니다.
        ParticleSystemRenderer[] particleSystemRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        
        foreach (ParticleSystemRenderer particleSystemRenderer in particleSystemRenderers) 
            _particleSystemRenderers.Add(particleSystemRenderer);
    }


    private void Update()
    {
        if (Follow != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, Follow.position, followSpeed);
            transform.position = newPosition;


            // 펫이 왼쪽 또는 오른쪽을 바라보도록 방향을 설정합니다.
            Vector3 direction = Follow.position - transform.position;
            if (direction.x >= 0)
            {
                foreach (ParticleSystemRenderer particleSystemRenderer in _particleSystemRenderers)
                    particleSystemRenderer.flip = RightDirection;
            }
            else
            {
                foreach (ParticleSystemRenderer particleSystemRenderer in _particleSystemRenderers)
                    particleSystemRenderer.flip = LeftDirection;
            }
        }
    }

    /// <summary>
    /// 강제로 펫을 이동시킵니다.
    /// </summary>
    public void ForceMove()
    {
        transform.position = Follow.position;
    }
}