using Unity.Netcode;
using UnityEngine;

public class SingleNetworkManager : UnityEngine.MonoBehaviour
{
    private void Start()
    {
        if (NetworkManager.Singleton != null)
            Destroy(gameObject);
    }
}
