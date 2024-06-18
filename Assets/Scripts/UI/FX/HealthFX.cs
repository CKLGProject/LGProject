using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct HealthGroup
{
    public Image HealthImage;
    public GameObject Icon;
}

public class HealthFX : MonoBehaviour
{
    [SerializeField] private HealthGroup[] lifePointUIList;

    [Tooltip("그레이 스케일 검은 강도")] [SerializeField]
    private float mixDarkIntensity;

    private readonly List<Material> _profileMaterialList = new();

    private static readonly int GrayScaleIntensity = Shader.PropertyToID("_Intensity");
    private static readonly int MixDarkIntensity = Shader.PropertyToID("_MixDarkIntensity");

    private void Start()
    {
        foreach (HealthGroup helHealthGroup in lifePointUIList)
        {
            Material material = Instantiate(helHealthGroup.HealthImage.material);
            helHealthGroup.HealthImage.material = material;
            _profileMaterialList.Add(material);

            helHealthGroup.Icon.SetActive(false);
        }
    }

    /// <summary>
    /// 생명 UI를 인자에 맞춰 싱크합니다.
    /// </summary>
    /// <param name="lifePoint">생명 포인트</param>
    public void SyncHealthUI(int lifePoint)
    {
        UpdateXIcon(lifePoint);
        UpdateImageGrayScale(lifePoint);
        UpdateMixDarkColor(lifePoint);
    }

    /// <summary>
    /// X 아이콘을 업데이트합니다.
    /// </summary>
    /// <param name="lifePoint"></param>
    private void UpdateXIcon(int lifePoint)
    {
        // X 아이콘 전부 숨깁니다.
        foreach (HealthGroup helHealthGroup in lifePointUIList)
            helHealthGroup.Icon.SetActive(false);

        int maxLifePoint = lifePointUIList.Length;
        int showIconCount = maxLifePoint - lifePoint;

        for (int i = 0; i < showIconCount; i++)
            lifePointUIList[i].Icon.SetActive(true);
    }

    /// <summary>
    /// 이미지의 그레이 스케일을 업데이트 합니다.
    /// </summary>
    /// <param name="lifePoint"></param>
    private void UpdateImageGrayScale(int lifePoint)
    {
        // 전부 일반 컬러로 만듭니다.
        foreach (Material material in _profileMaterialList)
            material.SetFloat(GrayScaleIntensity, 0);

        int maxLifePoint = lifePointUIList.Length;
        int showIconCount = maxLifePoint - lifePoint;

        // 피해를 입은 부분만 그레이 스케일로 만듭니다.
        for (int i = 0; i < showIconCount; i++)
            _profileMaterialList[i].SetFloat(GrayScaleIntensity, 1);
    }

    /// <summary>
    /// 이미지의 그레이 스케일을 업데이트 합니다.
    /// </summary>
    /// <param name="lifePoint"></param>
    private void UpdateMixDarkColor(int lifePoint)
    {
        // 전부 일반 컬러로 만듭니다.
        foreach (Material material in _profileMaterialList)
            material.SetFloat(MixDarkIntensity, 0);

        int maxLifePoint = lifePointUIList.Length;
        int showIconCount = maxLifePoint - lifePoint;

        // 피해를 입은 부분만 그레이 스케일로 만듭니다.
        for (int i = 0; i < showIconCount; i++)
            _profileMaterialList[i].SetFloat(MixDarkIntensity, mixDarkIntensity);
    }

    private void OnDestroy()
    {
        foreach (HealthGroup helHealthGroup in lifePointUIList)
            helHealthGroup.HealthImage.material = null;

        foreach (Material material in _profileMaterialList)
            Destroy(material);
    }
}