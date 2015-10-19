using UnityEngine;
using System;
using System.Collections;

public class PlayerShooter : Shooter
{

    Network.Session session;

    float moveLimit;

    public Action OnDead = () => { };

    void Awake()
    {
        moveLimit = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;

        if (MainSystem.Instance == null) return;
        session = MainSystem.Instance.Session;
    }

    void Update()
    {
        // Vector3でマウス位置座標を取得する
        var position = Input.mousePosition;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        var movePosition = Camera.main.ScreenToWorldPoint(position);

        if (Input.GetMouseButtonDown(0))
        {
            BulletFire(transform.localPosition.x, Vector2.up);

            if (session == null) return;
            session.SendBulletFire(transform.localPosition.x);
        }

        if (movePosition.x != transform.localPosition.x)
        {
            if (movePosition.x > -moveLimit && movePosition.x < moveLimit)
            {
                // ワールド座標に変換されたマウス座標を代入
                SetPosition(movePosition.x);

                if (session == null) return;
                session.SendPlayerPosition(transform.localPosition.x);
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (nowHP > 0) return;
        session.SendDead();
        OnDead();
    }
}
