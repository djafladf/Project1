using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostNova : Enemy
{
    [SerializeField] List<ParticleSystem> Particles;

    protected override void AttackMethod()
    {
        int CurDm = Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1]));
        GameManager.instance.BM.MakeMeele(new BulletInfo(CurDm, false, 0), 0.2f, transform.position, Vector2.zero, 0, true, im: Bull);
        Particles[0].Play();
    }
}
