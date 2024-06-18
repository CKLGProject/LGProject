using Data;
using UnityEngine;
using UnityEngine.Singleton;
using LGProject.CollisionZone;

namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }

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

        public Transform GetUserPlayer()
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

        /// <summary>
        /// 유저 캐릭터에 알맞게 유저 캐릭터를 생성합니다.
        /// </summary>
        /// <param name="characterType"></param>
        public void InitUserCharacter(ECharacterType characterType)
        {
                switch (characterType)
                {
                    case ECharacterType.Hit:
                        OnlinePlayer = playable[0];
                        break;
                    case ECharacterType.Frost:
                        OnlinePlayer = playable[1];
                        break;
                    case ECharacterType.Kane:
                        OnlinePlayer = playable[2];
                        break;
                    case ECharacterType.None:
                    case ECharacterType.Storm:
                    case ECharacterType.E:
                    default:
#if UNITY_EDITOR
                        Debug.LogError("유저 캐릭터 설정에 이슈가 발생하였습니다.");        
#endif
                        OnlinePlayer = playable[0];
                        break;
                }
                
                OnlinePlayer.transform.name = "Player";
                OnlinePlayer.gameObject.SetActive(true);
        }

        /// <summary>
        /// AI의 정보를 불러와 생성해주는 코드
        /// 예시는 위처럼 작업해주길 바람.
        /// </summary>
        public void InitAICharacter(ECharacterType characterType)
        {
            // 어...떻게 적으라는 거죠?? 😫
            
//             switch (characterType)
//             {
//                 case ECharacterType.Hit:
//                     OnlinePlayer = playable[0];
//                     break;
//                 case ECharacterType.Frost:
//                     OnlinePlayer = playable[1];
//                     break;
//                 case ECharacterType.Kane:
//                     OnlinePlayer = playable[2];
//                     break;
//                 case ECharacterType.None:
//                 case ECharacterType.Storm:
//                 case ECharacterType.E:
//                 default:
// #if UNITY_EDITOR
//                     Debug.LogError("유저 캐릭터 설정에 이슈가 발생하였습니다.");        
// #endif
//                     OnlinePlayer = playable[0];
//                     break;
//             }
//                 
//             OnlinePlayer.transform.name = "Player";
//             OnlinePlayer.gameObject.SetActive(true);
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
            
            // 아래 코드 활성화 시켜줘야함.
            //foreach(var item in AI)
            //{
            //    item.SetActive(false);
            //}
            
            ECharacterType userCharacterType = Singleton.Instance<GameManager>().GetCharacter(ActorType.User);
            ECharacterType aiCharacterType = Singleton.Instance<GameManager>().GetCharacter(ActorType.AI);
            
            InitUserCharacter(userCharacterType);
            InitAICharacter(aiCharacterType);
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