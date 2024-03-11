using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platinum :PlayerSetting
{
    [SerializeField] Sprite Bullet;

    protected override void Awake()
    {
        base.Awake();
        player.CurHP = player.MaxHP = 100;
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
            Vector2 Sub = (TargetPos.position - transform.position).normalized;
            float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
            if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
            for (int i = -ProjNum+1; i <= ProjNum-1; i++)
            {
                GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * DamageRatio), Penetrate, 1,
                transform.position, new Vector3(Mathf.Cos(rad + 0.25f * i), Mathf.Sin(rad + 0.25f * i)),
                10, Bullet, false, false);
            }
        }
    }
    float DamageRatio = 5f;
    int Penetrate = 0;
    int ProjNum = 1;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio++; break;
            case 2: Penetrate++; break;
            case 3: DamageRatio ++; break;
            case 4: ProjNum++; break;
            case 5: DamageRatio ++; break;
            case 6:
                Penetrate = 100;
                break;
        }
        return player.WeaponLevel;
    }
}
