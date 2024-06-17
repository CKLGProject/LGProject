using UnityEngine;
using UnityEngine.Singleton;
using LGProject.CollisionZone;

namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }
        Data.ECharacterType characterType;
        public GameObject[] playable;
        public GameObject[] AI;

        public GameObject OnlinePlayer;
        public CameraZone CameraZone;

        public int PlayCount { get; private set; }


        public bool IsStart { get; private set; }
        public bool IsEnd { get; private set; }
        public float Timer { get; private set; }

        /*****************************************************
         * public Methods
         * **/

        public Transform GetPlayers()
        {
            try
            {
                return OnlinePlayer.transform;
            }
            catch
            {
                return null;
            }
        }

        public void SetPlayers()
        {
            try
            {
                switch (characterType)
                {
                    case Data.ECharacterType.None:
                        Debug.Log("캐릭터 없음. Hit가 소환됩니다.");
                        OnlinePlayer = playable[0];
                        break;
                    case Data.ECharacterType.Hit:
                        OnlinePlayer = playable[0];
                        break;
                    case Data.ECharacterType.Frost:
                        OnlinePlayer = playable[1];
                        break;
                    case Data.ECharacterType.Kane:
                        OnlinePlayer = playable[2];
                        break;
                    case Data.ECharacterType.Storm:
                        Debug.Log("캐릭터 없음. Hit가 소환됩니다.");
                        OnlinePlayer = playable[0];
                        break;
                    case Data.ECharacterType.E:
                        Debug.Log("캐릭터 없음. Hit가 소환됩니다.");
                        OnlinePlayer = playable[0];
                        break;
                    default:
                        break;
                }
                OnlinePlayer.transform.name = "Player";
                OnlinePlayer.gameObject.SetActive(true);
            }
            catch
            {
                Debug.Log("설정된 픒레이어가 없음");
                OnlinePlayer = playable[0];
                OnlinePlayer.transform.name = "Player";
                OnlinePlayer.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// AI의 정보를 불러와 생성해주는 코드
        /// 예시는 위처럼 작업해주길 바람.
        /// </summary>
        public void SetAi()
        {

        }

        /*****************************************************
         * Private Methods
         * **/

        private void Awake()
        {
            if (Instance == null) 
                Instance = this;
            else
                Destroy(gameObject);
            characterType = Singleton.Instance<GameManager>().GetCharacter(Data.ActorType.User);

            if (PlayerPrefs.HasKey("PlayCount"))
            {
                PlayerPrefs.GetInt("PlayCount", PlayCount);
            }
            else
            {
                Debug.Log("Not Have Data");
                PlayCount = 1;
            }

            foreach (var item in playable)
            {
                item.SetActive(false);
            }
            foreach(var item in AI)
            {
                item.SetActive(false);
            }



            SetPlayers();
            SetAi();
        }

        private void Start()
        {

        }

        /// <summary>
        /// 게임을 시작시킵니다.
        /// </summary>
        public void GameStart()
        {
            IsStart = true;
            Timer = 180;
            // 시작하면 카메라를 옮겨줌
            CameraZone.ForcusPlayer(OnlinePlayer.transform);
            PlayCount = 2;
            
        }

        /// <summary>
        /// 게임을 종료시킵니다.
        /// </summary>
        public void GameEnd()
        {
            IsStart = false;
            IsEnd = true;
            // 게임이 종료되면 게임 횟수를 체크하여 난이도별 AI를 삽입한다.
            // 패턴은 약 - 약 - 강 현재는 이렇게 설정 예정.
            PlayerPrefs.SetInt("PlayCount", PlayCount);
        }
    }
}