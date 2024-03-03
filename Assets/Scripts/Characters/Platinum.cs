using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platinum :PlayerSetting
{
    [SerializeField] Sprite Bullet;

    protected override void Awake()
    {
        base.Awake();
        AttackInf.AttackSpeed = 1f;
        AttackInf.FirstDelay = 0.5f;
        AttackInf.LastDelay = 0.25f;
        player.CurHP = player.MaxHP = 50;
        player.CurSP = player.MaxSP = 10;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //print(CanMove);

    }

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * DamageRatio), Penetrate, 1,
                transform.position, (TargetPos.position - transform.position).normalized,
                10, Bullet, false, false);
        }
    }
    float DamageRatio = 5f;
    int Penetrate = 0;


    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio++; break;
            case 2: Penetrate++; break;
            case 3: DamageRatio ++; break;
            case 4: Penetrate++; break;
            case 5: DamageRatio ++; break;
            case 6:
                Penetrate = 100;
                break;
        }
        print($"{CharName} : {player.WeaponLevel}");
        return player.WeaponLevel;
    }
}
