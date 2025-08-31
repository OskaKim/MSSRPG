using Unity.Netcode;
using UnityEngine;

public class ServerBootstrap : MonoBehaviour
{
    private void Start()
    {
        if (!Application.isBatchMode)
        {
            Destroy(this);
            return;
        }

        Debug.Log("[SERVER] 데디케이티드 서버 시작 중...");

        // 클라이언트 연결 이벤트를 구독하여 플레이어 스폰 등 처리
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;

        // 서버 시작
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("[SERVER] NetworkManager.StartServer() 호출 성공.");
        }
        else
        {
            Debug.LogError("[SERVER] NetworkManager.StartServer() 호출 실패. NetworkManager 설정을 확인하세요.");
        }

        Destroy(this);
    }

    private static void HandleClientConnected(ulong clientId)
    {
        Debug.Log($"[SERVER] 클라이언트 {clientId} 연결됨.");
    }

    private static void OnServerStarted()
    {
        Debug.Log("[SERVER] 서버가 성공적으로 시작되었습니다. 클라이언트 연결 대기 중...");
    }

    private static void OnTransportFailure()
    {
        Debug.LogError("[SERVER] 서버 전송 계층 오류 발생! 포트 충돌, 방화벽 문제 등을 확인하세요.");
    }
}