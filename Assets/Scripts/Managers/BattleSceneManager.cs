using UnityEngine;


namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }
        private BattleModel _battleModel;

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
        }

        /// <summary>
        /// 게임을 시작시킵니디다.
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
        }
    }
}