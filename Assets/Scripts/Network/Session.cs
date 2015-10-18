using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Session
{
    // シリアライズ
    static MsgPack.ObjectPacker packer = new MsgPack.ObjectPacker();

    static byte[] Pack<Type>(Type data)
    {
        return packer.Pack(data);
    }

    static Type UnPack<Type>(byte[] data)
    {
        return packer.Unpack<Type>(data);
    }

    TcpListener listener;
    TcpClient tcpClient;

    NetworkStream stream;

    byte[] ReceiveBuffer = new byte[1024];

    public Action AcceptConnect = () => { };
    public Action OnCloseSession = () => { };
    public Action<Msg> OnRecvMessage = (msg) => { };

    // サーバーに接続に行く
    public Session(string host, int port)
    {
        tcpClient = new TcpClient(AddressFamily.InterNetwork);
        IPAddress[] remoteHost = Dns.GetHostAddresses(host);

        Debug.Log(string.Format("Connection Request to {0}:{1}", remoteHost[0], port));

        tcpClient.BeginConnect(remoteHost[0], port, new AsyncCallback(ConnectCallback), tcpClient);
    }

    void ConnectCallback(IAsyncResult result)
    {
        tcpClient = (TcpClient)result.AsyncState;
        tcpClient.EndConnect(result);
        Debug.Log(string.Format("Connection to {0}:{1}",
            ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address,
            ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port));

        stream = tcpClient.GetStream();
        AcceptConnect();
        BeginReceive();
    }


    // クライアントからの接続街
    public Session(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();

        Debug.Log(string.Format("Listen Start to {0}:{1}", IPAddress.Parse(UnityEngine.Network.player.ipAddress), port));

        listener.BeginAcceptTcpClient(
            new AsyncCallback(ListenerCallback),
            listener);
    }

    void ListenerCallback(IAsyncResult result)
    {
        listener = (TcpListener)result.AsyncState;
        tcpClient = listener.EndAcceptTcpClient(result);

        Debug.Log(string.Format("Connection to {0}:{1}",
            ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address,
            ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port));

        listener.Stop();

        stream = tcpClient.GetStream();
        AcceptConnect();
        BeginReceive();
    }

    // Sessionを切断
    public void Close()
    {
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }
        if (tcpClient != null)
        {
            tcpClient.Close();
        }
        OnCloseSession();
    }

    // 送信
    void Send<Type>(ProtocolType type, Type data)
    {
        if (stream == null || !stream.CanWrite)
        {
            Close();
            return;
        }
        Send(Pack(new Msg(type, Pack(data))));
    }

    void Send(Msg msg)
    {
        if (stream == null || !stream.CanWrite)
        {
            Close();
            return;
        }
        Send(Pack(msg));
    }

    void Send(byte[] msg)
    {
        try
        {
            tcpClient.Client.Send(msg);
        }
        catch (SocketException e)
        {
            Debug.LogError(e);
            Close();
        }
    }

    // 受信
    void BeginReceive()
	{
		try
		{
			if (!stream.CanRead) return;
			ReceiveBuffer = new byte[1024];

			stream.BeginRead(
				ReceiveBuffer,
				0,
				ReceiveBuffer.Length,
				new AsyncCallback(ReceiveDataCallback),
				stream
			);
		}
		catch(SocketException e)
		{
            Close();
			Debug.LogError(e);
		}
	}

	void ReceiveDataCallback(IAsyncResult result)
	{
		try
		{
			// 読み込んだバイト数を取得
			int bytes = stream.EndRead(result);

			//切断されたか調べる
			if (bytes <= 0)
			{
				Close();
				return;
			}

			// 読み込んだデータを表示
			var msg = UnPack<Msg>(ReceiveBuffer);
			OnRecvMessage(msg);

			BeginReceive();
		}
		catch (SocketException e)
		{
            Close();
			Debug.LogError(e);
		}
	}

	public void DoProtocol(Msg msg)
	{
		switch (msg.type)
		{
			case ProtocolType.GotoMain:
				Application.LoadLevelAsync("main");
				break;

			case ProtocolType.GotoTitle:
				Application.LoadLevelAsync("title");
				break;

			case ProtocolType.TextOnly:
				OnReceiveTextMessage(UnPack<TextMessage>(msg.data));
				break;

			case ProtocolType.PlayerPosition:
				OnReceivePlayerPosition(UnPack<PlayerPosition>(msg.data));
				break;

			case ProtocolType.BulletFire:
				OnReceiveBulletFire(UnPack<BulletFire>(msg.data));
				break;

            case ProtocolType.Dead:
                OnReceiveEnemyDead();
                break;
		}
	}

	public Action<TextMessage> OnReceiveTextMessage = (msg) => { };
	public void SendTextMessage(TextMessage msg)
	{
		Send(ProtocolType.TextOnly, msg);
	}

	public Action<PlayerPosition> OnReceivePlayerPosition = (msg) => { };
	public void SendPlayerPosition(PlayerPosition msg)
	{
		Send(ProtocolType.PlayerPosition, msg);
	}

	public Action<BulletFire> OnReceiveBulletFire = (msg) => { };
	public void SendBulletFire(BulletFire msg)
	{
		Send(ProtocolType.BulletFire, msg);
	}

    public Action OnReceiveEnemyDead = () => { };
    public void SendDead()
    {
        Send(new Msg(ProtocolType.Dead));
    }
}