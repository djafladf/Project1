using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEater : PlayerSetting
{
    [SerializeField] Sprite Spec;
    int Power = 1;

    protected override void AttackMethod()
    {
        Vector3 Gap = (transform.position - TargetPos.position).normalized * 0.2f;

        GameManager.instance.BM.MakeMeele(
            new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10),false,Power), 0.2f, 
            TargetPos.position + Gap, -player.Dir, 0, false);
    }



    public void SpecialAttack()
    {
        Vector3 Gap = (transform.position - TargetPos.position).normalized * 0.2f;
        GameManager.instance.BM.MakeMeele(
            new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, Power), 0.2f,
            TargetPos.position + Gap, -player.Dir, 0, false,Spec);
    }

    float DamageRatio = 2f;


    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: Power++; break;
            case 3: DamageRatio ++; break;
            case 4: Power++; break;
            case 5: DamageRatio ++; break;
            case 6: player.anim.SetBool("IsSpecial",true); break;
        }
        return player.WeaponLevel;
    }
}
