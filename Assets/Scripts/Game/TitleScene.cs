using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;

public class TitleScene : MonoBehaviour
{

	[SerializeField]
	Text serverAddressText;
	[SerializeField]
	Button serverStartButton;

	[SerializeField]
	InputField clientAddress;
	[SerializeField]
	Button clientStartButto;

	[SerializeField]
	GameObject darken;

	void Awake()
	{
		darken.SetActive(false);
	}

	void Start()
	{
		serverAddressText.text = IPAddress.Parse(UnityEngine.Network.player.ipAddress).ToString();
		clientAddress.text = serverAddressText.text;

		serverStartButton.onClick.AddListener(ServerStart);
		clientStartButto.onClick.AddListener(ClientStart);
	}

	void ServerStart()
	{
		darken.SetActive(true);
		MainSystem.Instance.ListenStart();
	}

	void ClientStart()
	{
		darken.SetActive(true);
		MainSystem.Instance.ConnectRequest(clientAddress.text);
	}
}