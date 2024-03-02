using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    GameObject[] Bullets;
    Bullet[] BulletScripts;
    SpriteRenderer[] BulletSprites;

    private void Awake()
    {
        Bullets = new GameObject[100];
        BulletScripts = new Bullet[100];
        BulletSprites = new SpriteRenderer[100];
        for (int i = 0; i < 100; i++)
        {
            Bullets[i] = Instantiate(Bullet, transform);
            Bullets[i].gameObject.SetActive(false);
            BulletScripts[i] = Bullets[i].GetComponent<Bullet>();
            BulletSprites[i] = Bullets[i].GetComponent<SpriteRenderer>();
        }
    }

    public void MakeBullet(int Damage, int Penetrate, float AfterTime,Vector3 Start, Vector3 Dir, float speed,Sprite im,bool IsMeele,bool IsEnemy)
    {
        for(int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletSprites[i].sprite = im;
                BulletScripts[i].Init(Damage, Penetrate, Dir * speed,IsMeele,IsEnemy, AfterTime);
                break;
            }
        }
    }
}
