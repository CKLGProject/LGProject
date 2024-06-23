using Data;
using FMODPlus;
using UnityEngine;
using LGProject.CollisionZone;
using UnityEngine.Serialization;
using UnityEngine.Singleton;

namespace LGProject
{
    public class BattleSceneSystem : MonoBehaviour
    {
        public static BattleSceneSystem Instance { get; private set; }

        [SerializeField] private GameObject[] playableList;

        [SerializeField] private GameObject[] aiList;

        [SerializeField] private GameObject[] petList;

        public GameObject OnlinePlayer { get; private set; }
        public CameraZone CameraZone;

        [field: Header("VocaFX")]
        [field: SerializeField]
        public VocaFX UserVocaFX { get; private set; }

        [field: SerializeField] public VocaFX AIVocaFX { get; private set; }

        [Header("Audio")] [SerializeField] private FMODAudioSource bgmAudioSource;

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
                PlayerPrefs.GetInt("PlayCount", PlayCount);
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("Not Have Data");
#endif
                PlayCount = 1;
            }

            // 유저 캐릭터 전부 비활성화
            foreach (GameObject playable in playableList)
                playable.SetActive(false);

            // AI 캐릭터 전부 비활성화
            foreach (GameObject ai in aiList)
                ai.SetActive(false);

            // 유저와 AI 캐릭터 활성화 진행
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
            CameraZone.FocusPlayer(OnlinePlayer.transform);
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

            // 게임 끝 소리 출력
            bgmAudioSource.SetParameter("Game End", 1);
        }

        /// <summary>
        /// 유저 캐릭터에 알맞게 캐릭터를 생성합니다.
        /// </summary>
        /// <param name="characterType">활성화 시킬 캐릭터 타입</param>
        private void InitUserCharacter(ECharacterType characterType)
        {
            switch (characterType)
            {
                case ECharacterType.Hit:
                    OnlinePlayer = playableList[0];
                    break;
                case ECharacterType.Frost:
                    OnlinePlayer = playableList[1];
                    break;
                case ECharacterType.Kane:
                    OnlinePlayer = playableList[2];
                    break;
                case ECharacterType.None:
                case ECharacterType.Storm:
                case ECharacterType.E:
                default:
#if UNITY_EDITOR
                    Debug.LogError("유저 캐릭터 설정에 이슈가 발생하였습니다.");
#endif
                    OnlinePlayer = playableList[0];
                    break;
            }

            OnlinePlayer.name = "Player";
            OnlinePlayer.gameObject.SetActive(true);

            // FX 할당
            UserVocaFX.FollowTarget = OnlinePlayer.transform;

            // 펫 생성
            EPetType petType = Singleton.Instance<GameManager>().GetPetType(ActorType.User);

            if (petType != EPetType.None)
            {
                int petIndex = (int)petType - 1;
                GameObject petObject =
                    Instantiate(petList[petIndex], OnlinePlayer.transform.position, Quaternion.identity);
                petObject.SetActive(true);
                petObject.name = "User Pet";
                if (petObject.TryGetComponent(out PetFollow petFollow))
                {
                    Transform petPoint = OnlinePlayer.transform.Find("Pet Point");
                    petFollow.Follow = petPoint;
                }
            }
        }

        /// <summary>
        /// AI에 알맞게 캐릭터를 생성합니다.
        /// </summary>
        /// <param name="characterType">활성화 시킬 캐릭터 타입</param>
        private void InitAICharacter(ECharacterType characterType)
        {
            // AI 캐릭터 생성
            GameObject targetAI;
            switch (characterType)
            {
                case ECharacterType.Hit:
                    targetAI = aiList[0];
                    break;
                case ECharacterType.Frost:
                    targetAI = aiList[1];
                    break;
                case ECharacterType.Kane:
                case ECharacterType.None:
                case ECharacterType.Storm:
                case ECharacterType.E:
                default:
#if UNITY_EDITOR
                    Debug.LogError("유저 캐릭터 설정에 이슈가 발생하였습니다.");
#endif
                    targetAI = aiList[0];
                    break;
            }

            targetAI.name = "AI";
            targetAI.SetActive(true);

            // FX 할당
            AIVocaFX.FollowTarget = targetAI.transform;

            // 펫 생성
            EPetType petType = Singleton.Instance<GameManager>().GetPetType(ActorType.AI);
            if (petType != EPetType.None)
            {
                int petIndex = (int)petType - 1;
                GameObject petObject = Instantiate(petList[petIndex], targetAI.transform.position, Quaternion.identity);
                petObject.SetActive(true);
                petObject.name = "AI Pet";
                if (petObject.TryGetComponent(out PetFollow petFollow))
                {
                    Transform petPoint = targetAI.transform.Find("Pet Point");
                    petFollow.Follow = petPoint;
                }
            }
        }
    }
}