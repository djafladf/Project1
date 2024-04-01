using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEater : PlayerSetting
{
    [SerializeField] Sprite Spec;

    protected override void AttackMethod()
    {
        Vector3 Gap = (transform.position - TargetPos.position).normalized * 0.2f;

        GameManager.instance.BM.MakeMeele(Mathf.FloorToInt(GameManager.instance.PlayerStatus.attack), 20, 0.2f, TargetPos.position + Gap, -player.Dir, 0, null, false);
    }

    public void SpecialAttack()
    {
        GameManager.instance.BM.MakeEffect(0.2f, TargetPos.position, Vector3.zero, Spec);
    }
}
