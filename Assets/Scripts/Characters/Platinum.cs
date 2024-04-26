using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platinum :PlayerSetting
{
    [SerializeField] Sprite Bullet;
    [SerializeField] BulletLine BL;

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
            int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);
            for (int i = -ProjNum+1; i <= ProjNum-1; i++)
            {
                GameManager.instance.BM.MakeBullet(
                    new BulletInfo(Damage,false,0,ignoreDefense : DefenseIgnore),Penetrate,
                transform.position, new Vector3(Mathf.Cos(rad + 0.25f * i), Mathf.Sin(rad + 0.25f * i)),
                15, false,Bullet,BL:BL);
            }
        }
    }
    float DamageRatio = 3f;
    float DefenseIgnore = 0;
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
                DefenseIgnore = 0.5f;
                break;
        }
        return player.WeaponLevel;
    }
}
