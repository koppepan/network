using UnityEngine;
using System.Collections;

namespace Network
{
    public enum ProtocolType
    {
        GotoMain,
        GotoTitle,
        TextOnly,
        PlayerPosition,
        BulletFire,
        SyncHitPoint,
        Dead,
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
        public Msg(ProtocolType type)
        {
            this.type = type;
            data = null;
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

    public struct SyncHitPoint
    {
        public int hp;
        public SyncHitPoint(int hp)
        {
            this.hp = hp;
        }
    }
}