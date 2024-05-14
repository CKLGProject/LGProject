using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        
    }
}
