using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


namespace LGProject
{

    public class InGameManager : MonoBehaviour
    {
        private static InGameManager instance;
        public static InGameManager Instance
        {
            get
            {
                return instance;
            }
        }

        public LGProject.PlayerState.Playable LeftPlayer;
        public LGProject.PlayerState.Playable RightPlayer;
        public bool IsStart = false;

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
            if (instance == null)
                instance = this;
            GameStart().Forget();
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        async UniTaskVoid GameStart()
        {
            //InGameUIManager.Instance.CountText.text = $"<rotate=\"0\">{3}</rotate>";
            //await UniTask.Delay(System.TimeSpan.FromSeconds(1));
            for (int i = 3; i > 0; --i)
            {
                InGameUIManager.Instance.CountText.text = $"<rotate=\"0\">{i}</rotate>";
                Debug.Log($"{i}");
                await UniTask.Delay(System.TimeSpan.FromSeconds(1));
            }
            InGameUIManager.Instance.CountText.text = $"<rotate=\"0\">Start!!</rotate>";
            IsStart = true;
            await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
            InGameUIManager.Instance.CountText.gameObject.SetActive(false);

        }
    }

}