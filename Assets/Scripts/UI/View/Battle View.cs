using Cysharp.Threading.Tasks;
using Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.Serialization;
using UnityEngine.Singleton;
using UnityEngine.UI;


public class BattleView : MonoBehaviour
{
    [Header("Poup View")] [SerializeField] private GameObject winPopup;
    [SerializeField] private GameObject losePopup;

    [Header("Health FX")] [SerializeField] private HealthFX userHealthFX;
    [SerializeField] private HealthFX aiHealthFX;

    [Header("Text")] [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI aiNameText;
    [SerializeField] private TextMeshProUGUI countText;

    [Header("Percent")]
    [SerializeField] private PercentageFX userDamageGageFX;
    [SerializeField] private PercentageFX aiDamageGageFX;

    [Header("EnergyBar")]
    [SerializeField] private Image userEnergyBar;
    [SerializeField] private Image aiEnergyBar;
    [SerializeField] private Image userEnergyIcon;
    [SerializeField] private Image aiEnergyIcon;
    [SerializeField] private Sprite ultimateReadySprite;
    [SerializeField] private Sprite ultimateReadyYetSprite;

    [FormerlySerializedAs("homeSceneLoader")]
    [Header("Home Scene")]
    [SerializeField] private SceneLoader lobbySceneLoader;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    [Header("StartLogo")]
    [SerializeField] private Image StartImage;

    [SerializeField] private ChangeImage[] userChangeImages;
    [SerializeField] private ChangeImage[] aiChangeImages;

    /// <summary>
    /// 초기 체인지 이미지를 바인딩합니다.
    /// </summary>
    public void InitBindChangeImage()
    {
        foreach (ChangeImage userChangeImage in userChangeImages)
        {
            ECharacterType userCharacter = Singleton.Instance<GameManager>().GetCharacter(ActorType.User);
            userChangeImage.SetCharacterType(userCharacter);
        }
        
        foreach (ChangeImage aiChangeImage in aiChangeImages)
        {
            ECharacterType aiCharacter = Singleton.Instance<GameManager>().GetCharacter(ActorType.AI);
            aiChangeImage.SetCharacterType(aiCharacter);
        }
    }
    
    /// <summary>
    /// 뷰를 전부 안보이도록 처리합니다.
    /// </summary>
    public void AllHideView()
    {
        losePopup.SetActive(false);
        winPopup.SetActive(false);
    }

    /// <summary>
    /// 에너지바 UI를 업데이트 합니다.
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="value">에너지 값</param>
    public void UpdateEnergyBarUI(ActorType actorType, float value)
    {
        switch (actorType)
        {
            case ActorType.User:
                userEnergyBar.fillAmount = value / 100;
                break;
            case ActorType.AI:
                aiEnergyBar.fillAmount = value / 100;
                break;
        }
    }

    /// <summary>
    /// 에너지바가 가득 차면 UI를 업데이트 합니다.
    /// </summary>
    public void UpdateEnergyIconUI(ActorType actorType, bool ready)
    {
        switch (actorType)
        {
            case ActorType.User:
                userEnergyIcon.sprite = ready ? ultimateReadySprite : ultimateReadyYetSprite;
                break;
            case ActorType.AI:
                aiEnergyIcon.sprite = ready ? ultimateReadySprite : ultimateReadyYetSprite;
                break;
        }
    }

    /// <summary>
    /// 라이프 포인트 UI를 업데이트 합니다.
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="lifePoint">라이프 포인트</param>
    public void UpdateLifPointUI(ActorType actorType, int lifePoint)
    {
        switch (actorType)
        {
            case ActorType.User:
                userHealthFX.SyncHealthUI(lifePoint);
                break;
            case ActorType.AI:
                aiHealthFX.SyncHealthUI(lifePoint);
                break;
        }
    }

    /// <summary>
    /// 데미지 게이지 UI를 업데이트 합니다.
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="damageGage">데미지 게이지</param>
    public void UpdateDamageGageUI(ActorType actorType, float damageGage)
    {
        switch (actorType)
        {
            case ActorType.User:
                userDamageGageFX.UpdateDamageGageText(damageGage);
                break;
            case ActorType.AI:
                aiDamageGageFX.UpdateDamageGageText(damageGage);
                break;
        }
    }

    /// <summary>
    /// Lose UI를 활성화 합니다.
    /// </summary>
    public void ShowLosePopup()
    {
        losePopup.SetActive(true);
    }

    /// <summary>
    /// Win UI를 활성화 합니다.
    /// </summary>
    public void ShowWinPopup()
    {
        winPopup.SetActive(true);
    }

    /// <summary>
    /// 타이머를 체크하여 게임 시간을 알려주는 Text에 기록합니다.
    /// </summary>
    public void SetTimerText(float value)
    {
        timerText.text = $"{(int)value}";
    }


    /// <summary>
    /// 이름 텍스트를 value로 변경합니다.
    /// </summary>
    /// <param name="actorType">타겟 액터</param>
    /// <param name="value">변경할 이름</param>
    public void SetNameText(ActorType actorType, string value)
    {
        switch (actorType )
        {
            case ActorType.User:
                userNameText.text = value;
                break;
            case ActorType.AI:
                aiNameText.text = value;
                break;
        }
    }

    /// <summary>
    /// 카운트 다운 텍스트를 업데이트합니다.
    /// </summary>
    /// <param name="count">카운트</param>
    public void UpdateCountDownUI(int count)
    {
        if (count > 0)
            countText.text = $"<rotate=\"0\">{count}</rotate>";
        else
        {
            //countText.text = $"<rotate=\"0\">Start!!</rotate>";
            DisappearCountTextAfterOneSecond().Forget();
        }
    }

    private async UniTaskVoid DisappearCountTextAfterOneSecond()
    {
        countText.gameObject.SetActive(false);
        StartImage.gameObject.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

        StartImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 홈 씬으로 이동합니다.
    /// </summary>
    public void GoHome()
    {
        lobbySceneLoader.AllowCompletion();
    }
}