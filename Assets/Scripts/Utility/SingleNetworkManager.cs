using Unity.Netcode;
using UnityEngine;

public class SingleNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null)
            Destroy(gameObject);
    }
}
