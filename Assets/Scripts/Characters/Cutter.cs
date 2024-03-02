using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : PlayerSetting
{
    [SerializeField] Sprite NormalAttack;
    [SerializeField] Sprite Bullet;

    protected override void Awake()
    {
        base.Awake();
        AttackInf.AttackSpeed = 2f;
        AttackInf.FirstDelay = 1;
        AttackInf.LastDelay = 0.5f;
        player.CurHP = player.MaxHP = 50;
        player.CurSP = player.MaxSP = 10;
    }

    protected override void AttackMethod()
    {
        if (TargetPos != null)
            GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * 3),0,0.3f,
                transform.position, -player.Dir,
                0, NormalAttack, true,false);
    }

    
}

