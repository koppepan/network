using UnityEngine;
using System;
using System.Collections;

public class PlayerShooter : Shooter
{
    enum Vector
    {
        None,
        Right,
        Left,
    };

    [SerializeField]
    float moveSpeed = 0.1f;

    Network.Session session;

    float moveLimit;
    Vector moveVector = Vector.None;

    bool rightPush = false;
    bool leftPush = false;

    public Action OnDead = () => { };

    void Awake()
    {
        moveLimit = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - (transform.localScale.x / 2);

        if (MainSystem.Instance == null) return;
        session = MainSystem.Instance.Session;
    }

    void Update()
    {
        MoveUpdate();
    }

    public void PushRightButton()
    {
        rightPush = true;
        moveVector = Vector.Right;
    }
    public void ReleaseRightButton()
    {
        rightPush = false;
        if(leftPush)
        {
            moveVector = Vector.Left;
            return;
        }
        moveVector = Vector.None;
    }
    public void PushLeftButton()
    {
        leftPush = true;
        moveVector = Vector.Left;
    }
    public void ReleaseLeftButton()
    {
        leftPush = false;
        if(rightPush)
        {
            moveVector = Vector.Right;
            return;
        }
        moveVector = Vector.None;
    }

    void MoveUpdate()
    {
        if (moveVector == Vector.None) return;

        var move = Vector3.right * moveSpeed * (moveVector == Vector.Right ? 1 : -1);
        var afterPos = transform.localPosition.x + move.x; 

        if (afterPos < -moveLimit || afterPos > moveLimit) return;

        transform.localPosition += move;

        if (session == null) return;
        session.SendPlayerPosition(transform.localPosition.x);
    }

    public void BulletFire()
    {
        BulletFire(transform.localPosition.x, Vector2.up);

        if (session == null) return;
        session.SendBulletFire(transform.localPosition.x);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (nowHP > 0) return;
        session.SendDead();
        OnDead();
    }
}
