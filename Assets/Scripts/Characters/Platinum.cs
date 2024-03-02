using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platinum :PlayerSetting
{
    [SerializeField] Sprite Bullet;

    protected override void Awake()
    {
        base.Awake();
        CanMove = true;
        AttackInf.AttackSpeed = 1f;
        AttackInf.FirstDelay = 0.5f;
        AttackInf.LastDelay = 0.25f;
        player.CurHP = player.MaxHP = 50;
        player.CurSP = player.MaxSP = 10;
    }

    protected override void AttackMethod()
    {
        if (TargetPos != null)
            GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * 6),1,1, 
                transform.position,(TargetPos.position - transform.position).normalized,
                10, Bullet, false, false);
    }
}
