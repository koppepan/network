using UnityEngine;
using System.Collections;

namespace Network
{
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

	public struct SyncHitPoint
	{
		public int hp;
		public SyncHitPoint(int hp)
		{
			this.hp = hp;
		}
	}
}