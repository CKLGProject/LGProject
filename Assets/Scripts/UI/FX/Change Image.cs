using Cysharp.Threading.Tasks;
using Data;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 캐릭터에 따라 다른 이미지가 보이도록 처리해주는 컴포넌트입니다.
/// </summary>
public class ChangeImage : MonoBehaviour
{
    public ECharacterType CharacterType { get; private set; }

    [ArrayElementTitle("CharacterType")] public CharacterByImage[] characterByImages;

    public Image[] targetImages;

    private void Start()
    {
        ChangeImageAsync().Forget();
    }

    private async UniTaskVoid ChangeImageAsync()
    {
        await UniTask.Yield();
        UpdateChangeImage();
    }

    /// <summary>
    /// 체인지 이미지를 업데이트 합니다.
    /// </summary>
    public void UpdateChangeImage()
    {
        foreach (CharacterByImage characterByImage in characterByImages)
        {
            if (characterByImage.CharacterType == CharacterType)
            {
                foreach (Image targetImage in targetImages)
                {
                    targetImage.sprite = characterByImage.CharacterProfileImage;
                    targetImage.transform.localScale = characterByImage.isFlip ? new Vector3(-1, 1, 1) : Vector3.one;
                }
            }
        }
    }

    /// <summary>
    /// 캐릭터 타입을 설정합니다.
    /// </summary>
    /// <param name="characterType">캐릭터 타입</param>
    public void SetCharacterType(ECharacterType characterType)
    {
        CharacterType = characterType;
    }
}

[Serializable]
public class CharacterByImage
{
    public ECharacterType CharacterType;
    public Sprite CharacterProfileImage;
    public bool isFlip;
}