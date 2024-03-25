using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Skulslr : Enemy
{

    protected override void FixedUpdate()
    {
        if (!IsLive || OnIce) return;
        rigid.velocity = Vector2.zero;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && !anim.GetBool("IsAttack"))
        {
            AttackPos = Target.position;
            MoveAble = false;
            anim.SetBool("IsRange",Vector2.Distance(AttackPos, transform.position) > 1);
            anim.SetBool("IsAttack", true);
        }
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        if (BeginAttack)
        {
            AttackPos = Target.position;
            anim.SetBool("IsRange", Vector2.Distance(AttackPos, transform.position) > 1);
        }
    }
}
