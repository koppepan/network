using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainSystem : MonoBehaviour {

	Session session;
	public Session Session { get { return session; } }

	static MainSystem instance;
	public static MainSystem Instance
	{
		get
		{
			if (instance == null) instance = GameObject.FindObjectOfType<MainSystem>();
			return instance;
		}
	}

	Queue<Msg> msgQueue = new Queue<Msg>();

	// インスタンスが存在するか？
	static bool existsInstance = false;
	void Awake()
	{
		// インスタンスが存在するなら破棄する
		if (existsInstance)
		{
			Destroy(gameObject);
			return;
		}

		// 存在しない場合
		// 自身が唯一のインスタンスとなる
		existsInstance = true;
		DontDestroyOnLoad(gameObject);
	}

	void OnApplicationQuit()
	{
		CloseSession();
	}

	public void ListenStart()
	{
		session = new Session(25000);
		session.AcceptConnect = StartSession;
	}

	public void ConnectRequest(string address)
	{
		session = new Session(address, 25000);
		session.AcceptConnect = StartSession;
	}

	void StartSession()
	{
		session.OnRecvMessage += ReceiveMsg;
		session.OnCloseSession += OnCloseSession;
		msgQueue.Enqueue(new Msg(ProtocolType.GotoMain, null));
	}

	void CloseSession()
	{
		msgQueue.Enqueue(new Msg(ProtocolType.GotoTitle, null));

		if (session == null) return;
		session.OnRecvMessage -= ReceiveMsg;
		session.OnCloseSession -= OnCloseSession;
	}

	void OnCloseSession()
	{
		Debug.Log("切断されました。");
		CloseSession();
	}

	public void ReceiveMsg(Msg msg)
	{
		msgQueue.Enqueue(msg);
	}

	void Update()
	{
		// Queueが空になるまで内容をDequeue
		while (0 < msgQueue.Count)
		{
			session.DoProtocol(msgQueue.Dequeue());
		}
	}
}
