using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using USingleton.SelfSingleton;


namespace LGProject
{
    public class BattleSceneManager : MonoBehaviour
    {
        public static BattleSceneManager Instance { get; private set; }
        private BattleModel _battleModel;

        public bool IsStart { get; private set; }

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
        }
        
    }
}