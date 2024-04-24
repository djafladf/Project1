using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEater : PlayerSetting
{
    [SerializeField] Sprite Spec;
    int Power = 3;

    protected override void AttackMethod()
    {
        Vector3 Gap = (transform.position - TargetPos.position).normalized * 0.2f;

        GameManager.instance.BM.MakeMeele(
            new BulletInfo(Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack * 10),false,Power),0.2f, 
            TargetPos.position + Gap, -player.Dir, 0, false);
    }

    public void SpecialAttack()
    {
        //GameManager.instance.BM.MakeEffect(0.2f, TargetPos.position, Vector3.zero, Spec);
    }
}
