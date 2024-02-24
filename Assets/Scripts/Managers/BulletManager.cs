using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    GameObject[] Bullets;
    Bullet[] BulletScripts;

    private void Awake()
    {
        Bullets = new GameObject[100];
        BulletScripts = new Bullet[100];
        for(int i = 0; i < 100; i++)
        {
            Bullets[i] = Instantiate(Bullet, transform);
            Bullets[i].gameObject.SetActive(false);
            BulletScripts[i] = Bullets[i].GetComponent<Bullet>();
        }
    }

    public void MakeBullet(Transform Start, Vector3 Dir, float speed)
    {
        for(int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start.position;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletScripts[i].Init(Dir * speed);
                break;
            }
        }
    }
}
