using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


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
        for(int i = 1; i < 4; i++)
        {
            Debug.Log($"{i}");
            await UniTask.Delay(1);
        }
        Debug.Log("Start!!");
        IsStart = true;

    }
}
