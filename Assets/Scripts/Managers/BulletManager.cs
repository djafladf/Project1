using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class BulletManager : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject Warning;
    GameObject[] Bullets;
    GameObject[] Warnings;
    WarningBullet[] WarningScripts;
    Bullet[] BulletScripts;
    BulletInfo[] BulletInfos;

    private void Awake()
    {
        GameManager.instance.BM = this;
        GameManager.instance.StartLoading();
    }

    public void Init()
    {
        Bullets = new GameObject[400];
        BulletScripts = new Bullet[400];
        BulletInfos = new BulletInfo[400];
        Warnings = new GameObject[40];
        WarningScripts = new WarningBullet[40];
        for(int i = 0; i < 40; i++)
        {
            Warnings[i] = Instantiate(Warning, transform);
            Warnings[i].SetActive(false);
            WarningScripts[i] = Warnings[i].GetComponent<WarningBullet>();
        }

        for (int i = 0; i < 400; i++)
        {
            Bullets[i] = Instantiate(Bullet, transform);
            Bullets[i].gameObject.SetActive(false);
            Bullets[i].name = $"{i}";
            BulletScripts[i] = Bullets[i].GetComponent<Bullet>();
        }
        GameManager.instance.StartLoading();
    }

    DeBuff NoDeBuff = new DeBuff();
    Buff NoBuff = new Buff();

    public void MakeWarning(Vector3 Pos, float time, Vector2 size, Color S, System.Action<Vector3> act)
    {
        for(int i = 0; i < 40; i++)
        {
            if (!Warnings[i].activeSelf)
            {
                Warnings[i].SetActive(true);
                Warnings[i].transform.position = Pos;
                WarningScripts[i].Init(time, size, act,S);
                break;
            }
        }
    }

    /// <summary>
    /// 원거리
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
    public void MakeBullet(BulletInfo Info, int Penetrate, Vector3 Start, Vector3 Dir, float speed, bool IsEnemy,
        Sprite im = null, DeBuff debuffInfo = null,BulletLine BL = null, RuntimeAnimatorController anim = null, float delay = 0)
    {
        for(int i = 0; i < 400; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                if (Info.SpeedFactor != 0) speed *= Info.SpeedFactor;
                BulletScripts[i].Init_Attack(Penetrate, Dir * speed,false,IsEnemy,0,Info.ScaleFactor,im,BL,anim,delay:delay);
                BulletInfos[i] = Info;
                break;
            }
        }
    }

    /// <summary>
    /// 근거리
    /// </summary>
    /// <param name="Info"> Info </param>
    /// <param name="AfterTime"> Last Time </param>
    /// <param name="Start"> Start Pos </param>
    /// <param name="Dir"> Moving Dir(norm) </param>
    /// <param name="im"> Image Of Bullet(none = null) </param>
    /// <param name="IsEnemy"> Is Enemy </param>
    /// <param name="debuffInfo"> About DeBuff </param>
    public void MakeMeele(BulletInfo Info, float AfterTime, Vector3 Start, Vector3 Dir, float speed, bool IsEnemy, Sprite im = null, RuntimeAnimatorController Anim = null, DeBuff debuffInfo = null,float delay = 0)
    {
        for (int i = 0; i < 400; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletInfos[i] = Info;
                if (Info.SpeedFactor != 0) speed *= Info.SpeedFactor;
                BulletScripts[i].Init_Attack(-1, Dir * speed, true, IsEnemy, AfterTime, Info.ScaleFactor, im, Anim:Anim,delay:delay);
                break;
            }
        }
    }

    public void MakeBoom(BulletInfo Info, BulletInfo After, Vector3 Start, Vector3 Dir, float Speed, Sprite im, Sprite HitIm, bool IsEnemy, DeBuff debuffInfo = null,BulletLine BL = null,float delay = 0)
    {
        for (int i = 0; i < 400; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletInfos[i] = Info;
                BulletScripts[i].Init_Explode(After, Dir * Speed,IsEnemy,im,HitIm,BL : BL,delay:delay);
                break;
            }
        }
    }


    public void MakeEffect(float AfterTime, Vector3 Start, Vector3 Dir, float speed,Sprite im, bool AlphaChange=true,BulletLine BL = null, RuntimeAnimatorController Anim = null)
    {
        for (int i = 0; i < 400; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start; Dir.z = 0;
                Bullets[i].transform.rotation = Quaternion.FromToRotation(Vector3.up, Dir);
                BulletScripts[i].Init_Effect(AfterTime,im,Dir * speed,AlphaChange,BL,Anim);
                break;
            }
        }
    }

    public void MakeBuff(BulletInfo BI,Vector3 Start, Sprite im, bool IsEnemy,bool IsField = false)
    {
        for (int i = 0; i < 400; i++)
        {
            if (!Bullets[i].activeSelf)
            {
                Bullets[i].SetActive(true);
                Bullets[i].transform.position = Start;
                Bullets[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                BulletScripts[i].Init_Buff(BI.ScaleFactor,im,IsEnemy,IsField);
                BulletInfos[i] = BI;
                break;
            }
        }
    }

    public BulletInfo GetBulletInfo(int ind)
    {
        return BulletInfos[ind];
    }
}
