using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class VocaFX : MonoBehaviour
{
    [SerializeField] private Texture2D[] fuckTextureList;
    [SerializeField] private Texture2D[] tangTextureList;
    [SerializeField] private Texture2D[] kungTextureList;

    [SerializeField] private Transform[] textTargetList;
    private MeshRenderer[] _textRenderers;

    [Header("Target")]
    public Transform FollowTarget;
    
    [Header("Setting")]
    [Tooltip("페이드 지속 시간")]
    [SerializeField] private float fadeDuration = 0.5f;
    
    [Tooltip("페이드를 시전하는 딜레이")]
    [SerializeField] private float fadeDelay = 0.25f;

    private Transform _cameraTransform;
    
    private List<int> playingListCach;

    private MaterialPropertyBlock _materialPropertyBlock;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private static readonly int Color = Shader.PropertyToID("_Color");

    private void Start()
    {
        // 매쉬 렌더러를 할당하는 작업을 진행합니다.
        Init();
    }

    /// <summary>
    /// 매쉬 렌더러를 할당하는 작업을 진행합니다.
    /// </summary>
    private void Init()
    {
        _cameraTransform = Camera.main.transform;
        
        // 메모리 공간 바인딩
        int textRendererCount = textTargetList.Length;
        _textRenderers = new MeshRenderer[textRendererCount];
        _materialPropertyBlock = new MaterialPropertyBlock();
        playingListCach = new List<int>();

        // 할당
        for (int i = 0; i < textRendererCount; i++)
            _textRenderers[i] = textTargetList[i].GetComponent<MeshRenderer>();

        // 비활성화 처리
        foreach (Transform textTarget in textTargetList)
            textTarget.gameObject.SetActive(false);
    }

    /// <summary>
    /// 의성이 타입에 맞춰서 텍스트 FX를 보이게 합니다.
    /// </summary>
    /// <param name="vocaType"></param>
    public void PlayVoca(EVocaType vocaType)
    {
        // 겹치는지 검사해서 겹치면 다른 값을 가져옵니다.
        int playIndex = CalculatePlayIndex(Random.Range(0, textTargetList.Length));

        // 만약 -1이면 표시할 수 없음
        if (playIndex == -1)
            return;
        
        // 해당 텍스트를 보이게 합니다.
        textTargetList[playIndex].gameObject.SetActive(true);

        Texture2D targetTexture;
        switch (vocaType)
        {
            case EVocaType.Kung:
                int randomKungTextureIndex = Random.Range(0, kungTextureList.Length);
                targetTexture = kungTextureList[randomKungTextureIndex];
                break;
            case EVocaType.Tang:
                int randomTangTextureIndex = Random.Range(0, tangTextureList.Length);
                targetTexture = tangTextureList[randomTangTextureIndex];
                break;
            case EVocaType.Fuck:
                int randomFuckTextureIndex = Random.Range(0, fuckTextureList.Length);
                targetTexture = fuckTextureList[randomFuckTextureIndex];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(vocaType), vocaType, null);
        }

        // 텍스처 바인딩
        _textRenderers[playIndex].material.SetTexture(MainTex, targetTexture);

        // FX
        _textRenderers[playIndex].material
            .DOFade(0, fadeDuration)
            .SetDelay(fadeDelay)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                _textRenderers[playIndex].material.SetColor(Color, UnityEngine.Color.white);
                textTargetList[playIndex].gameObject.SetActive(false);
                playingListCach.Remove(playIndex);
            })
            .Play();

        // 클리어
        _materialPropertyBlock.Clear();
    }

    private int CalculatePlayIndex(int index)
    {
        // 표시할 Text가 없다면 -1 표시
        if (playingListCach.Count == textTargetList.Length)
            return -1;
        
        if (playingListCach.Contains(index))
        {
            int randomTextIndex = Random.Range(0, textTargetList.Length);
            return CalculatePlayIndex(randomTextIndex); // 재귀 호출의 결과를 반환
        }

        playingListCach.Add(index); // 새로운 인덱스를 캐시에 추가
        return index;  
    }

    private void Update()
    {
        // 빌보드 처리
        UpdateBillboardEffect();
        
        if (!FollowTarget)
            return;
        
        transform.position = FollowTarget.position;
    }
    
    /// <summary>
    /// 글자가 카메라를 향해 빌보드를 합니다.
    /// </summary>
    private void UpdateBillboardEffect()
    {
        Assert.IsNotNull(_cameraTransform, "_cameraTransform != null");

        foreach (Transform textTarget in textTargetList)
        {
            textTarget.LookAt(_cameraTransform);
            
            Vector3 rotation = textTarget.rotation.eulerAngles;
            rotation.y += 180;
            textTarget.rotation = Quaternion.Euler(rotation);
        }
    }
}