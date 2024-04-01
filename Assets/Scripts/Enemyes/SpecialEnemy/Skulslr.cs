using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Skulslr : Enemy
{

    [SerializeField] Sprite Boom;
    [SerializeField] Sprite Bullet;

    protected override void FixedUpdate()
    {
        if (!IsLive || OnIce) return;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            if(!OnHit)rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && !anim.GetBool("IsAttack"))
        {
            AttackPos = Target.position;
            MoveAble = false;
            anim.SetBool("IsRange",Vector2.Distance(AttackPos, transform.position) > 2f);
            anim.SetBool("IsAttack", true);
        }
    }

    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeBoom(10, 25, 0, transform.position, (AttackPos - transform.position).normalized, 10, Bullet, Boom, true);
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        if (BeginAttack)
        {
            AttackPos = Target.position;
            anim.SetBool("IsRange", Vector2.Distance(AttackPos, transform.position) > 2f);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable(); GameManager.instance.UM.BossName.text = "½ºÄÃ½´·¹´õ";
    }

    protected override void HPChange()
    {
        GameManager.instance.UM.BossHP.fillAmount = HP / (float)MaxHP;
    }
}
