using UnityEngine;
using System.Collections;

public enum ProtocolType
{
	GotoMain,
	GotoTitle,
	TextOnly,
	PlayerPosition,
	BulletFire,
};

public struct Msg
{
	public ProtocolType type;
	public byte[] data;

	public Msg(ProtocolType type, byte[] data)
	{
		this.type = type;
		this.data = data;
	}
}

public struct TextMessage
{
	public string text;
	public TextMessage(string text)
	{
		this.text = text;
	}
}

public struct PlayerPosition
{
	public float pos;
	public PlayerPosition(float pos)
	{
		this.pos = pos;
	}
}

public struct BulletFire
{
	public float pos;
	public BulletFire(float pos)
	{
		this.pos = pos;
	}
}