using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System;
using System.Threading;

public class SocketManager : MonoBehaviour
{
    private ClientWebSocket _sockets = null;

    private void Start() {
        Connection();
    }

    private async void Connection()
    {
        Debug.Log("커넥션 시작");
        if(_sockets != null && _sockets.State == WebSocketState.Open)
        {
            Debug.Log("이미 연결되어있는 소켓입니다.");
            return;
        }

        _sockets = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:50000");

        await _sockets.ConnectAsync(serverUri, CancellationToken.None);

        Debug.Log("연결 완료");

        ArraySegment<byte> bufferSegment = new ArraySegment<byte>(new byte[1024]);
        WebSocketReceiveResult result = await _sockets.ReceiveAsync(bufferSegment, CancellationToken.None);

        string msg = System.Text.Encoding.UTF8.GetString(bufferSegment.Array);
        Debug.Log(msg);
    }

    private List<string> msg;
    
    async void ReceiveLoop()
    {
        // 무한루프 돌면서 ReceiveAsync 를 실행해
        // 받았으면 맴버변수인 리스트에다가 받은 것을 넣어줘
    }

    private void OnDestroy() {
        Disconnect();
    }

    public void Disconnect()
    {
        if(_sockets != null)
        {
            _sockets.CloseAsync(WebSocketCloseStatus.NormalClosure, "Quit", CancellationToken.None);
        }
    }
}
