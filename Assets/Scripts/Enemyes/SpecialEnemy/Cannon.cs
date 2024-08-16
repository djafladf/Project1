using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Cannon : Enemy
{
    [SerializeField] Sprite Boom;
    [SerializeField] ParticleSystem BoomEf;

    Vector3 Sub1 = new Vector3(1, 1, 0);
    Vector3 Sub2 = new Vector3(-1, 1, 0);
    void SpecialAttack()
    {
        BoomEf.transform.localPosition = spriteRenderer.flipX ? Sub1 : Sub2;
        BoomEf.Play();
        GameManager.instance.BM.MakeWarning(AttackPos, 1f, Boom.bounds.size * 0.8f, Color.red, SpecialAttackSub);
    }

    void SpecialAttackSub(Vector3 pos)
    {
        GameManager.instance.BM.MakeMeele(new BulletInfo((int)(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1])), false, 0, ignoreDefense: 0.25f), 0.3f, pos, Vector3.zero, 0, true, Boom);
    }
}
