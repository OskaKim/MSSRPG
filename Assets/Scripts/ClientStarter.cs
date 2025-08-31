using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

// 타이틀 화면에서는 아직 통신을 시작하지 않기 때문에, 게임씬에서 사용.
public class ClientStarter : MonoBehaviour
{
    [SerializeField] private string serverIpAddress = "127.0.0.1";
    [SerializeField] private ushort serverPort = 7777;

    // 타이틀화면을 거치지 않고, 바로 게임화면에서 시작하면 오프라인 모드로 실행됨.
    public static bool IsOfflineMode = true;

    public static ulong MyClientId { get; private set; } = 0;

    private void Start()
    {
        if (Application.isBatchMode)
        {
            Destroy(this);
            return;
        }

        if (IsOfflineMode)
        {
            SetupAsOfflineMode();
            return;
        }

        // UnityTransport 설정
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(serverIpAddress, serverPort);

        Debug.Log($"[CLIENT] 서버 {serverIpAddress}:{serverPort} 에 연결 시도 중...");

        // 클라이언트 연결 관련 이벤트 구독
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailureClient;

        // 클라이언트 시작
        var success = NetworkManager.Singleton.StartClient();
        Debug.Log($"[CLIENT] NetworkManager.StartClient() 호출 결과: {success}");

        if (!success)
        {
            Debug.LogError("[CLIENT] 클라이언트 시작 요청 실패. NetworkManager 설정을 확인하세요.");
        }

        Destroy(this);
    }

    private void SetupAsOfflineMode()
    {
        Debug.Log("[OFFLINE] 오프라인 모드로 시작합니다. 로컬 호스트 실행.");

        // 클라이언트가 아닌 호스트로 시작
        var success = NetworkManager.Singleton.StartHost();
        if (success)
        {
            MyClientId = NetworkManager.Singleton.LocalClientId;
            Debug.Log($"[OFFLINE] 호스트 시작 성공! 내 클라이언트 ID: {MyClientId}");
        }
        else
        {
            Debug.LogError("[OFFLINE] 호스트 시작 실패! NetworkManager 설정을 확인하세요.");
        }

        Destroy(this);
    }

    private static void OnClientStarted()
    {
        Debug.Log("[CLIENT] 클라이언트가 시작되었습니다. 서버 응답 대기 중...");
    }

    private static void OnClientConnected(ulong clientId)
    {
        MyClientId = clientId;

        Debug.Log($"[CLIENT] 서버에 성공적으로 연결되었습니다! 내 클라이언트 ID: {clientId}");
    }

    private static void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[CLIENT] 서버에서 연결이 끊어졌습니다. 클라이언트 ID: {clientId}");
    }

    private static void OnTransportFailureClient()
    {
        Debug.LogError("[CLIENT] 클라이언트 전송 계층 오류 발생! 서버 주소/포트, 방화벽, 네트워크를 확인하세요.");
    }
}