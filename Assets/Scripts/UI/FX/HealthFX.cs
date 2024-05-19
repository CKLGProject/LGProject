using System;
using UnityEngine;

public class HealthFX : MonoBehaviour
{
    [SerializeField] private GameObject[] lifePointUIList;

    /// <summary>
    /// 생명 UI를 인자에 맞춰 싱크합니다.
    /// </summary>
    /// <param name="lifePoint">생명 포인트</param>
    public void SyncHealthUI(int lifePoint)
    {
        foreach (GameObject o in lifePointUIList) 
            o.SetActive(false);

        for (int i = 0; i < lifePoint; i++) 
            lifePointUIList[i].SetActive(true);
    }
}