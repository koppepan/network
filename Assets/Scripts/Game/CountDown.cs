using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CountDown : MonoBehaviour {

	[SerializeField]
	Text text;

	public Action OnFinish = () => { };

	void Start()
	{
		StartCoroutine(WaitOneSecond());
	}

	IEnumerator WaitOneSecond()
	{
		text.text = "3";
		yield return new WaitForSeconds(1f);
		text.text = "2";
		yield return new WaitForSeconds(1f);
		text.text = "1";
		yield return new WaitForSeconds(1f);
		text.text = "Game Start!";
		yield return new WaitForSeconds(1f);

		OnFinish();
		Destroy(gameObject);
	}
}
