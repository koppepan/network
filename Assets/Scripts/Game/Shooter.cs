using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Shooter : MonoBehaviour {

    [SerializeField]
    GameObject BulletPrefab;

    List<GameObject> selfBullets = new List<GameObject>();

    public float PositionY { get; set; }

    [SerializeField]
    private int hp = 3;
    public int nowHP
    {
        get { return hp; }
    }

    public void SetPosition(float x)
    {
        transform.position = new Vector2(x, PositionY);
    }

    public void BulletFire(float x, Vector2 vec)
    {
        var obj = Instantiate(BulletPrefab, new Vector2(x, PositionY), Quaternion.identity) as GameObject;
        selfBullets.Add(obj);

        obj.GetComponent<Bullet>().SetVector(vec);
    }

    private void OnTriggerEnter(Collider other)
    {
        selfBullets.RemoveAll(x => x == null);
        if (selfBullets.Any(x => x == other.gameObject)) return;

        Destroy(other.gameObject);

        hp--;
        if (hp != 0) return;
        Debug.LogWarning("game over");
    }
}
