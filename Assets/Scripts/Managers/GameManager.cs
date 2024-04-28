using UnityEngine;
using USingleton.AutoSingleton;

[Singleton(nameof(GameManager))]
public class GameManager : MonoBehaviour
{
    public string Nickname { get; set; }
}
