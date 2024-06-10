using UnityEngine;


namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }

        public bool IsStart { get; private set; }
        public bool IsEnd { get; private set; }
        public float Timer { get; private set; }

        /*****************************************************
         * public Methods
         * **/
        public void SetPlayers()
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
            //FileManager.Instance.LoadData();

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