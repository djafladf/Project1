using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseDrone : Enemy
{
    protected override void FixedUpdate()
    {
        if (Vector3.Magnitude(transform.position - GameManager.instance.player.Self.position) < 10) return;
        base.FixedUpdate();
    }
    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeBuff(BI, transform.position, Bull, true,true);
    }
}
