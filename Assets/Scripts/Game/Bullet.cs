using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    [SerializeField]
    float speed = 0.5f;

	Vector2 vector;

	public void SetVector(Vector2 vec)
	{
		vector = vec * speed;
	}

	void Update () {

		transform.localPosition += new Vector3(vector.x, vector.y, 0);

		if (transform.localPosition.y > 10 || transform.localPosition.y < -10)
		{
			Destroy(gameObject);
		}
	}
}
