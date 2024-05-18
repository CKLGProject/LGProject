using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace LGProject
{
    public class InGameUIManager : MonoBehaviour
    {
        private static InGameUIManager instance;
        public static InGameUIManager Instance
        {
            get
            {
                return instance;
            }
        }

        public GameObject EndGamePanel;

        public GameObject WinImage;
        public GameObject LoseImage;

       
        public GameObject[] LeftPlayerLife;
        public GameObject[] RightPlayerLife;

        public TextMeshProUGUI CountText;

        [SerializeField] private Transform _leftPlayer;
        [SerializeField] private Transform _rightPlayer;

        //public List<GameObject>

        private void Awake()
        {
            if (instance == null)
                instance = this;

            InitPanel();
        }

        private void Update()
        {
            
        }

        private void InitPanel()
        {
            LoseImage.SetActive(false);
            WinImage.SetActive(false);

            EndGamePanel.SetActive(false);
        }

        public void LifeDown(Transform playerTransform, ref int Life)
        {
            if (playerTransform == _leftPlayer)
            {
                LeftPlayerLife[--Life].SetActive(false);
            }
            else if (playerTransform == _rightPlayer)
            {
                RightPlayerLife[--Life].SetActive(false);
            }
            else
                Debug.Log("Not have A Player");

            if (Life <= 0)
            {
                EndGamePanel.SetActive(true);
                if (playerTransform == _leftPlayer)
                {
                    LoseImage.SetActive(true);
                }
                else if(playerTransform == _rightPlayer)
                {
                    WinImage.SetActive(true);
                }
            }
        }

        private void PlayableLifeDown()
        {

        }
    }
}
