using R3;
using ReactiveTouchDown;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

public class LobbyView : MonoBehaviour
{
    [Header("Text")] [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI plugText;

    /// <summary>
    /// 매치 버튼
    /// </summary>
    [Header("Buttons")] [SerializeField] private GameObject matchButton;

    /// <summary>
    /// 모집 버튼
    /// </summary>
    [SerializeField] private GameObject recruitButton;

    /// <summary>
    /// 랭킹 버튼
    /// </summary>
    [SerializeField] private GameObject rankButton;

    /// <summary>
    /// 우편 버튼
    /// </summary>
    [SerializeField] private Button mailButton;

    /// <summary>
    /// 퀘스트 버튼
    /// </summary>
    [SerializeField] private Button questButton;

    /// <summary>
    /// 캐릭터 버튼
    /// </summary>
    [SerializeField] private Button characterButton;

    /// <summary>
    /// 친구
    /// </summary>
    [SerializeField] private Button friendButton;

    /// <summary>
    /// 인벤토리
    /// </summary>
    [SerializeField] private Button inventoryButton;

    /// <summary>
    /// 설정 버튼
    /// </summary>
    [SerializeField] private Button settingButton;

    [Tooltip("미구현 메세지")] [SerializeField] private string errorMessage;

    [Header("Event")]
    public UnityEvent OnClickRanking;
    public UnityEvent OnClickMatch;
    public UnityEvent OnClickRecruit;

    /// <summary>
    /// 매치 버튼 옵저버입니다.
    /// </summary>
    /// <returns>MatchButton Observer</returns>
    public Observable<Unit> MatchButtonAsObservable()
    {
        return matchButton.TouchDownAsObservable();
    }

    /// <summary>
    /// 모집 버튼 옵저버입니다.
    /// </summary>
    /// <returns>RecruitButton Observer</returns>
    public Observable<Unit> RecruitButtonAsObservable()
    {
        return recruitButton.TouchDownAsObservable();
    }

    /// <summary>
    /// 랭킹 버튼 옵저버입니다.
    /// </summary>
    /// <returns>RankButton Observer</returns>
    public Observable<Unit> RankButtonAsObservable()
    {
        return rankButton.TouchDownAsObservable();
    }

    /// <summary>
    /// 메일 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> MailButtonAsObservable()
    {
        return mailButton.OnClickAsObservable();
    }

    /// <summary>
    /// 퀘스트 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> QuestButtonAsObservable()
    {
        return questButton.OnClickAsObservable();
    }

    /// <summary>
    /// 캐릭터 버튼 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> CharacterButtonAsObservable()
    {
        return characterButton.OnClickAsObservable();
    }

    /// <summary>
    /// 친구 버튼 옵저버 입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> FriendButtonAsObservable()
    {
        return friendButton.OnClickAsObservable();
    }

    /// <summary>
    /// 인벤토리 옵저버입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> InventoryButtonAsObservable()
    {
        return inventoryButton.OnClickAsObservable();
    }

    /// <summary>
    /// 세팅 버튼 옵저버 입니다.
    /// </summary>
    /// <returns></returns>
    public Observable<Unit> SettingButtonAsObservable()
    {
        return settingButton.OnClickAsObservable();
    }

    /// <summary>
    /// 미구현 메세지를 띄웁니다.
    /// </summary>
    public void ShowErrorMessage()
    {
        LGUtility.Toast(errorMessage);
    }

    /// <summary>
    /// 닉네임을 설정합니다.
    /// </summary>
    /// <param name="nickName"></param>
    public void SetNickName(string nickName)
    {
        nickNameText.text = nickName;
    }

    /// <summary>
    /// 레벨을 설정합니다.
    /// </summary>
    /// <param name="level"></param>
    public void SetLevel(int level)
    {
        levelText.text = level.ToString();
    }

    /// <summary>
    /// 코인을 설정합니다.
    /// </summary>
    /// <param name="coin"></param>
    public void SetCoin(uint coin)
    {
        coinText.text = coin.ToString("N0");
    }

    /// <summary>
    /// 플러그를 설정합니다.
    /// </summary>
    /// <param name="plug"></param>
    public void SetPlug(uint plug)
    {
        plugText.text = plug.ToString("N0");
    }
}