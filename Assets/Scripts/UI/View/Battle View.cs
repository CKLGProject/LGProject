using Cysharp.Threading.Tasks;
using Data;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;


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

    /// <summary>
    /// 뷰를 전부 안보이도록 처리합니다.
    /// </summary>
    public void AllHideView()
    {
        losePopup.SetActive(false);
        winPopup.SetActive(false);
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
        losePopup.SetActive(true);
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
            countText.text = $"<rotate=\"0\">Start!!</rotate>";
            DisappearCountTextAfterOneSecond().Forget();
        }
    }

    private async UniTaskVoid DisappearCountTextAfterOneSecond()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        countText.gameObject.SetActive(false);
    }
}