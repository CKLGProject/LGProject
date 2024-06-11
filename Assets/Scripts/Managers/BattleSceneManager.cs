using UnityEngine;
using UnityEngine.Singleton;

namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }
        Data.ECharacterType characterType;
        public GameObject[] playable;

        public GameObject OnlinePlayer;


        public bool IsStart { get; private set; }
        public bool IsEnd { get; private set; }
        public float Timer { get; private set; }

        /*****************************************************
         * public Methods
         * **/

        public Transform GetPlayers()
        {
            return OnlinePlayer.transform;
        }

        public void SetPlayers()
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
                case Data.ECharacterType.Cane:
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
            SetPlayers();
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
        }

        /// <summary>
        /// 게임을 종료시킵니다.
        /// </summary>
        public void GameEnd()
        {
            IsStart = false;
            IsEnd = true;
            // 여기서 호출하고 싶은데..
            //FileManager.Instance.SaveData();
            
        }
    }
}