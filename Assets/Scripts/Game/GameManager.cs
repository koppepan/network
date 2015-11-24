using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    PlayerShooter player;
    [SerializeField]
    Shooter enemy;

    [SerializeField]
    GameObject gameOverPanel;
	[SerializeField]
	CountDown countDown;

    Network.Executor executor;


    void Awake()
    {
        if (MainSystem.Instance == null) return;

        executor = MainSystem.Instance.Executor;
        executor.OnReceiveTextMessage += OnReceiveTextMessage;
        executor.OnReceivePlayerPosition += OnReceivePlayerPosition;
        executor.OnReceiveBulletFire += OnReceiveBulletFire;
        executor.OnReceiveEnemyDead += OnReceiveEnemyDead;

        player.OnDead += OnGameOver;

		countDown.OnFinish = () => { player.IsFire = true; };
    }

    void Start()
    {
        var topLimit = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;

        player.PositionY = -topLimit + (transform.localScale.y / 2);
        player.SetPosition(0);

        enemy.PositionY = topLimit - (enemy.transform.localScale.y / 2);
        enemy.SetPosition(0);
    }

    void OnDestroy()
    {
        player.OnDead -= OnGameOver;

        executor.OnReceiveTextMessage -= OnReceiveTextMessage;
        executor.OnReceivePlayerPosition -= OnReceivePlayerPosition;
        executor.OnReceiveBulletFire -= OnReceiveBulletFire;
        executor.OnReceiveEnemyDead -= OnReceiveEnemyDead;
    }

    public void GotoTitle()
    {
        Application.LoadLevel("title");
    }

    void OnGameOver()
    {
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
        gameOverPanel.SetActive(true);
    }
}
