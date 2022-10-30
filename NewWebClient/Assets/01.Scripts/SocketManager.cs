using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.WebSockets;
using System;
using System.Threading;
using GGM.Proto.Tank;
using Google.Protobuf;

#region 주석

// public enum PacketCategory
// {
//     SendData = 1,
//     SendFloat = 2,
// }
// public class SendData
// {
//     public int a;
//     public int b;

//     public ArraySegment<byte> Serialize()
//     {
//         ushort cnt = 2;
//         ArraySegment<byte> segment = new ArraySegment<byte>(new byte[12]);

//         // 이 패킷의 코드를 기록한거야.
//         Array.Copy(BitConverter.GetBytes((ushort)PacketCategory.SendData), 0, segment.Array, segment.Offset + cnt, sizeof(ushort));

//         cnt += sizeof(ushort);

//         Array.Copy(BitConverter.GetBytes(this.a), 0, segment.Array, segment.Offset + cnt, sizeof(int));
//         cnt += sizeof(int);

//         Array.Copy(BitConverter.GetBytes(this.b), 0, segment.Array, segment.Offset + cnt, sizeof(int));
//         cnt += sizeof(int);

//         Array.Copy(BitConverter.GetBytes(cnt), 0, segment.Array, segment.Offset, sizeof(ushort));

//         return segment;
//     }
// }

// public class SendFloat
// {
//     public float a;
//     public float b;

//     public ArraySegment<byte> Serialize()
//     {
//         ushort cnt = 2;
//         ArraySegment<byte> segment = new ArraySegment<byte>(new byte[12]);

//         Array.Copy(BitConverter.GetBytes((ushort)PacketCategory.SendFloat), 0, segment.Array, segment.Offset + cnt, sizeof(ushort));

//         cnt += sizeof(ushort);

//         Array.Copy(BitConverter.GetBytes(this.a), 0, segment.Array, segment.Offset + cnt, sizeof(float));
//         cnt += sizeof(float);

//         Array.Copy(BitConverter.GetBytes(this.b), 0, segment.Array, segment.Offset + cnt, sizeof(float));
//         cnt += sizeof(float);

//         Array.Copy(BitConverter.GetBytes(cnt), 0, segment.Array, segment.Offset, sizeof(ushort));

//         return segment;
//     }
// }
#endregion


public class SocketManager : MonoBehaviour
{

    private ClientWebSocket _soket = null;
    private void Start()
    {
    
        Connection();
    }
    public async void Connection()
    {
        Debug.Log("커넥션 시작");
        if (_soket != null && _soket.State == WebSocketState.Open)
        {
            Debug.Log("이미 연결되어있는 소켓입니다");
            return;
        }
        _soket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:50000");

        await _soket.ConnectAsync(serverUri, CancellationToken.None);

        Debug.Log("연결 완료");

        // SendData a = new SendData { a = 2000, b = 15};

        // await _soket.SendAsync(a.Serialize(), WebSocketMessageType.Binary, true, CancellationToken.None);

        // ArraySegment<byte> bufferSegment = new ArraySegment<byte>(new byte[1024]);
        // WebSocketReceiveResult result = await _soket.ReceiveAsync(bufferSegment, CancellationToken.None);
        // string msg = System.Text.Encoding.UTF8.GetString(bufferSegment.Array);

        // Debug.Log(msg);
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.A))
        {
            // 여기서는 SendData를 보내고

            CMove cmove = new CMove { PlayerId = 1, X = 20f, Y = 30f };
            ushort len = (ushort)(cmove.CalculateSize() + 4);

            byte[] payload = cmove.ToByteArray();

            ArraySegment<byte> segment = new ArraySegment<byte>(new byte[len]);

            Array.Copy(BitConverter.GetBytes(len), 0, segment.Array, segment.Offset, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)MSGID.Cmvoe), 0, segment.Array, segment.Offset + 2, sizeof(ushort));
            Array.Copy(cmove.ToByteArray(), 0, segment.Array, segment.Offset + 4, len - 4);

            SendData(segment);

            // SendData s = new SendData{a = 2000, b = 15};
            // SendData(s.Serialize());

        }
        else if(Input.GetKeyDown(KeyCode.B))
        {
            // 여기서는 SendFloat를 보내서
            // 서버에서 알맞게 파싱해서 출력하도록 해봐
            // SendFloat s = new SendFloat{a = 24.88f, b = 15.5f};
            // SendData(s.Serialize());

        }
    }

    public async void SendData(ArraySegment<byte> segment)
    {
        await _soket.SendAsync(segment, WebSocketMessageType.Binary, true, CancellationToken.None);
    }


    private List<string> msg;
    // async void ReciveLoop()
    // {
    //     while(true)
    //     {
    //         //무한루프돌면서 리시브어싱크 실행
    //         // 받았으면 멤버변수인 리스트에다가 받은것을 넣어줘
    //     }
    // }
    //asyn 리시브 루프를 만들어서 와일트루로 야랄
    private void OnDestroy()
    {
        DisConnect();
    }

    public void DisConnect()
    {
        if (_soket != null)
        {
            _soket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Quit", CancellationToken.None);
        }
    }
}