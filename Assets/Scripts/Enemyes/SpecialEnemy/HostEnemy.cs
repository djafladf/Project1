using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostEnemy : Enemy
{
    protected override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(DotDamage());
    }

    protected override void AttackMethod()
    {
        if (IsRange)
        {
            BI.Damage = Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1]));
            GameManager.instance.BM.MakeBullet(BI, 0, transform.position, (AttackPos - transform.position).normalized, 16, true, Bull);
        }
        else
        {
            int CurDm = Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1]));
            GameManager.instance.BM.MakeMeele(new BulletInfo(CurDm, false, 0), 0.2f, AttackPos, Vector2.zero, 0, true, im: Bull);
        }
    }

    IEnumerator DotDamage()
    {
        int Damage = Mathf.FloorToInt(HP * 0.02f);
        while (IsLive)
        {
            HP -= Damage;
            if (HP <= 0)
            {
                anim.SetTrigger("Dead");
                StartCoroutine(DeadLater());
                spriteRenderer.sortingOrder = 1;
                IsLive = false; rigid.simulated = false; coll.enabled = false;
                GameManager.instance.UM.KillCountUp(1);
                GameManager.instance.ES.DeadCount(EnemyType);
            }

            yield return GameManager.OneSec;
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision) { }
}
