using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    Shooter player;
    [SerializeField]
    Shooter enemy;

    [SerializeField]
    GameObject gameOverPanel;

    Network.Session session;
    Network.Executor executor;

    float moveLimit;
    float topLimit;

    void Awake()
    {
        if (MainSystem.Instance == null) return;
        session = MainSystem.Instance.Session;

        executor = MainSystem.Instance.Executor;
        executor.OnReceiveTextMessage += OnReceiveTextMessage;
        executor.OnReceivePlayerPosition += OnReceivePlayerPosition;
        executor.OnReceiveBulletFire += OnReceiveBulletFire;
        executor.OnReceiveEnemyDead += OnReceiveEnemyDead;
    }

    void Start()
    {
        moveLimit = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        topLimit = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;

        player.PositionY = -topLimit + (player.transform.localScale.y / 2);
        player.SetPosition(0);
        player.OnDead += OnPlayerDead;

        enemy.PositionY = topLimit - (enemy.transform.localScale.y / 2);
        enemy.SetPosition(0);
    }

    void OnDestroy()
    {
        player.OnDead -= OnPlayerDead;

        if (session == null) return;
        executor.OnReceiveTextMessage -= OnReceiveTextMessage;
        executor.OnReceivePlayerPosition -= OnReceivePlayerPosition;
        executor.OnReceiveBulletFire -= OnReceiveBulletFire;
        executor.OnReceiveEnemyDead -= OnReceiveEnemyDead;
    }

    void Update()
    {
        // Vector3でマウス位置座標を取得する
        var position = Input.mousePosition;
        // マウス位置座標をスクリーン座標からワールド座標に変換する
        var movePosition = Camera.main.ScreenToWorldPoint(position);

        if (Input.GetMouseButtonDown(0))
        {
            player.BulletFire(player.transform.localPosition.x, Vector2.up);
            if (session == null) return;
            session.SendBulletFire(player.transform.localPosition.x);
        }

        if (movePosition.x != transform.localPosition.x)
        {
            if (movePosition.x > -moveLimit && movePosition.x < moveLimit)
            {
                // ワールド座標に変換されたマウス座標を代入
                player.SetPosition(movePosition.x);
                if (session == null) return;
                session.SendPlayerPosition(player.transform.localPosition.x);
            }
        }
    }

    void OnPlayerDead()
    {
        session.SendDead();
        gameOverPanel.SetActive(true);
    }


    void OnReceiveTextMessage(Network.TextMessage msg)
	{
		Debug.Log(msg.text);
	}

	void OnReceivePlayerPosition(Network.PlayerPosition msg)
	{
        enemy.SetPosition(msg.pos);
	}

	void OnReceiveBulletFire(Network.BulletFire msg)
	{
		enemy.BulletFire(msg.pos, Vector2.down);
	}

    void OnReceiveEnemyDead()
    {
        Debug.LogWarning("win");
        gameOverPanel.SetActive(true);
    }
}
