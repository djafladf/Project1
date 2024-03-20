using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Kazemaru : PlayerSetting
{
    [SerializeField] Sprite MeeleAttack;
    [SerializeField] Sprite bullet;
    [SerializeField] GameObject DollPref;

    GameObject Doll;

    protected override void Awake()
    {
        if (!IsSummon)
        {
            player.CurHP = player.MaxHP = 120;
            player.Defense = player.MaxDefense = 10;
            DamageRatio = 1f;
            SpecialRatio = 2f;
        }
        else
        {
            player.CurHP = player.MaxHP = 60;
            player.Defense = player.MaxDefense = 5;
            DamageRatio = 0.5f;
            SpecialRatio = 1f;
        }
        player.CurSP = player.MaxSP = 10;

        if(!IsSummon) Doll = Instantiate(DollPref, transform.parent);
        base.Awake();
    }

    protected override void AttackMethod()
    {
        if (Vector3.Distance(TargetPos.position, transform.position) <= 2)
        {
            GameManager.instance.BM.MakeMeele((int)(GameManager.instance.PlayerStatus.attack * DamageRatio),0,0.3f,
                transform.position, -player.Dir,
                0, MeeleAttack,false);
        }
        if (ProjNum != 0)
        {
            Vector2 Sub = (TargetPos.position - transform.position).normalized;
            float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
            if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
            for (int i = -ProjNum; i <= ProjNum; i++)
            {
                GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * DamageRatio * SpecialRatio), 0,0,
                transform.position, new Vector3(Mathf.Cos(rad + 0.1f * i), Mathf.Sin(rad + 0.1f * i), 0),
                15, bullet,false);
            }
        }


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsSummon) if(!Doll.activeSelf && player.WeaponLevel > 6){ Doll.SetActive(true); Doll.transform.position = transform.position + Vector3.right; }
    }

    float DamageRatio;
    float SpecialRatio;
    int ProjNum;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: DamageRatio += 0.75f; break;
            case 3: ProjNum++; AttackRange = 5; break;
            case 4: DamageRatio += 0.5f; break;
            case 5: DamageRatio += 0.75f; break;
            case 6: SpecialRatio = 3f; ProjNum++; break;
        }
        return player.WeaponLevel;
    }

    private void OnDisable()
    {
        if(!IsSummon) Doll.SetActive(false);
    }
}
