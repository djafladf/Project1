using System.Collections;
using UnityEngine;

public class Aurora : PlayerSetting
{

    protected override void Awake()
    {
        base.Awake();
        player.CurHP = player.MaxHP = 450;
        player.CurSP = player.MaxSP = 10;
        player.Defense = player.MaxDefense = 25;
    }

    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeMeele((int)(GameManager.instance.PlayerStatus.attack * DamageRatio), 0, 0.2f,
                    TargetPos.position, Vector3.zero, 0, null, false);
    }

    float DamageRatio = 1f;
}

