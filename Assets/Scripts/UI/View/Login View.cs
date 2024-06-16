using UnityEngine;
using UnityEngine.SceneSystem;
using UnityEngine.Singleton;

public class InitStartView : MonoBehaviour
{
    [SerializeField] private SceneLoader lobbySceneLoader;
    
    /// <summary>
    /// 로비 씬으로 연결합니다.
    /// </summary>
    public void Connect()
    {
        string nickName = PlayerPrefs.GetString("Nickname","Guest");
        Singleton.Instance<GameManager>().SetNickname(nickName);
        
        lobbySceneLoader.AllowCompletion();
    }
}