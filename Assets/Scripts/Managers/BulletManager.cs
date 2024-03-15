using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    GameObject[] Bullets;
    Bullet[] BulletScripts;
    BulletInfo[] BulletInfos;

    public void Init()
    {
        Bullets = new GameObject[100];
        BulletScripts = new Bullet[100];
        BulletInfos = new BulletInfo[100];
        for (int i = 0; i < 100; i++)
        {
            Bullets[i] = Instantiate(Bullet, transform);
            Bullets[i].gameObject.SetActive(false);
            Bullets[i].name = $"{i}";
            BulletScripts[i] = Bullets[i].GetComponent<Bullet>();
            BulletInfos[i] = new BulletInfo(0,false,0);
        }
    }

    DeBuff NoDeBuff = new DeBuff();
    Buff NoBuff = new Buff();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Damage"> Dmg </param>
    /// <param name="Penetrate"> Penetration </param>
    /// <param name="Power"> KnockBack Power </param>
    /// <param name="Start"> Start Pos </param>
    /// <param name="Dir"> Moving Dir(normaize) </param>
    /// <param name="speed"> speed </param>
    /// <param name="im"> Image Of Bullet </param>
    /// <param name="IsEnemy"> Is Enemy </param>
    /// <param name="debuffInfo"> About DeBuff </param>
    public void MakeBullet(int Damage, int Penetrate,float Power, Vector3 Start, Vector3 Dir, float speed,Sprite im,bool IsEnemy,DeBuff debuffInfo = null)
    {
        for(int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletInfos[i].Set(Damage, false, Power, debuffInfo);
                BulletScripts[i].Init_Attack(Damage, Penetrate, Dir * speed,false,IsEnemy,0,im);
                break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Damage"> Dmg </param>
    /// <param name="Power"> KnockBack Power</param>
    /// <param name="AfterTime"> Last Time </param>
    /// <param name="Start"> Start Pos </param>
    /// <param name="Dir"> Moving Dir(normalize) </param>
    /// <param name="speed"> speed </param>
    /// <param name="im"> Image Of Bullet(none = null) </param>
    /// <param name="IsEnemy"> Is Enemy </param>
    /// <param name="debuffInfo"> About DeBuff </param>
    public void MakeMeele(int Damage, float Power, float AfterTime, Vector3 Start, Vector3 Dir,float speed, Sprite im, bool IsEnemy, DeBuff debuffInfo = null)
    {
        for (int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletScripts[i].Init_Attack(Damage, -1, Dir * speed, true, IsEnemy, AfterTime, im);
                BulletInfos[i].Set(Damage, false, Power, debuffInfo);
                break;
            }
        }
    }

    public void MakeEffect(float AfterTime, Vector3 Start, Vector3 Dir, Sprite im)
    {
        for (int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletScripts[i].Init_Effect(AfterTime,im);
                break;
            }
        }
    }

    public void MakeBuff(Vector3 Start, Sprite im, Buff buffInfo, bool IsEnemy)
    {
        for (int i = 0; i < 100; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start;
                BulletScripts[i].Init_Buff(im,IsEnemy);
                BulletInfos[i].Set(0, false, 0, buffs : buffInfo);
                break;
            }
        }
    }

    public BulletInfo GetBulletInfo(int ind)
    {
        return BulletInfos[ind];
    }
}
