using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using MyBox;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using USingleton.SelfSingleton;

public class RelayManager : Singleton
{
    /// <summary>
    /// 최대 접속할 수 있는 클라이언트 수
    /// </summary>
    public int MaxConnections = 2;

    /// <summary>
    /// 릴레이를 사용할 수 있는 여부
    /// </summary>
    public bool IsRelayEnabled => Transport != null && Transport.Protocol == UnityTransport.ProtocolType.RelayUnityTransport;

    /// <summary>
    /// Join코드를 반환합니다.
    /// </summary>
    public string JoinCode { get; private set; }

    private UnityTransport Transport => NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();

    private Guid _hostAllocationID;
    private Guid _playerAllocationID;
    private string _allocationRegion = string.Empty;
    private string _playerID = "Not Signed In";

    protected override async void Awake()
    {
        base.Awake();
        
        // 초기화 처리 진행
        await UnityServices.InitializeAsync();
        
        // 로그인이 되면 접속 처리
        await OnSignIn();
    }

    /// <summary>
    /// 로그인 버튼을 클릭했을 때의 이벤트 핸들러입니다.
    /// </summary>
    public async UniTask<bool> OnSignIn()
    {
        if (AuthenticationService.Instance.IsSignedIn)
            return true;
        
        // 익명으로 로그인을 시도합니다.
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // 로그인에 성공했을 경우
        if (AuthenticationService.Instance.IsSignedIn)
        {
            // 플레이어의 ID를 가져옵니다.
            _playerID = AuthenticationService.Instance.PlayerId;
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// 릴레이 서버를 설정합니다. 
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> OnSetupRelayServer()
    {
        // 로그인 안되어 있으면 에러를 띄움
        if (!AuthenticationService.Instance.IsSignedIn)
            return false;

        // Unity 서비스에 최대 MaxConnections명의 플레이어를 처리할 릴레이 서버를 할당하도록 요청
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);


        _hostAllocationID = allocation.AllocationId; // 할당 ID를 저장합니다. 
        _allocationRegion = allocation.Region; // 현재 서버를 돌리고 있는 지역을 바인딩

        // 새로운 호스트 데이터를 생성합니다.  
        RelayHostData relayHostData = new() {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationId = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionData = allocation.ConnectionData,
            IPv4Address = allocation.RelayServer.IpV4
        };

        // 트랜스포트를 재 설정합니다.
        Transport.SetRelayServerData(relayHostData.IPv4Address, relayHostData.Port, relayHostData.AllocationIDBytes, relayHostData.Key, relayHostData.ConnectionData);

        return true;
    }

    /// <summary>
    /// Join Code를 가져옵니다. 
    /// </summary>
    public async UniTask<string> OnGetJoinCode()
    {
        JoinCode = await RelayService.Instance.GetJoinCodeAsync(_hostAllocationID);
        return JoinCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="joinCode"></param>
    /// <returns></returns>
    public async UniTask<bool> OnJoinRelay(string joinCode)
    {
        // 로그인 안되어 있으면 에러를 띄움
        if (!AuthenticationService.Instance.IsSignedIn)
            return false;
        
        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);

        Debug.Log(allocation);
        
        RelayJoinData relayJoinData = new RelayJoinData() {
            Key = allocation.Key,
            Port = (ushort)allocation.RelayServer.Port,
            AllocationId = allocation.AllocationId,
            AllocationIDBytes = allocation.AllocationIdBytes,
            ConnectionDataBytes = allocation.ConnectionData,
            HostConnectionData = allocation.HostConnectionData,
            Ipv4Address = allocation.RelayServer.IpV4,
            JoinCode = joinCode
        };

        Transport.SetRelayServerData(relayJoinData.Ipv4Address, relayJoinData.Port, relayJoinData.AllocationIDBytes, relayJoinData.Key, relayJoinData.ConnectionDataBytes, relayJoinData.HostConnectionData);

        JoinCode = relayJoinData.JoinCode;

        return true;
    }

    public void Reset()
    {
        // 모두 초기화
        _allocationRegion = string.Empty;
        JoinCode = string.Empty;
    }
}
