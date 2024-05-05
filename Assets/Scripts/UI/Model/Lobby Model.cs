using R3;
using UnityEngine;

public class LobbyModel : MonoBehaviour
{
    private ReactiveProperty<string> _nickname = new ReactiveProperty<string>();
    public Observable<string> NicknameObservable => _nickname;

    public string Nickname
    {
        get => _nickname.Value;
        set => _nickname.Value = value;
    }

    private ReactiveProperty<int> _level = new ReactiveProperty<int>();
    public Observable<int> LevelObservable => _level;

    public int Level
    {
        get => _level.Value;
        set => _level.Value = value;
    }

    private ReactiveProperty<uint> _coin = new ReactiveProperty<uint>();
    public Observable<uint> CoinObservable => _coin;

    public uint Coin
    {
        get => _coin.Value;
        set => _coin.Value = value;
    }

    private ReactiveProperty<uint> _plug = new ReactiveProperty<uint>();
    public Observable<uint> PlugObservable => _plug;

    public uint Plug
    {
        get => _plug.Value;
        set => _plug.Value = value;
    }
}