using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Revenger : Enemy
{
    [SerializeField] ParticleSystem FireSys;
    ParticleSystem.ShapeModule Fire;

    Vector3 Sub1 = new Vector3(1, -0.1f, 0);
    Vector3 Sub2 = new Vector3(-1, -0.1f, 0);

    protected override void Awake()
    {
        base.Awake();
        Fire = FireSys.shape;
    }

    new void FixedUpdate()
    {
        if (!IsLive || OnIce) return;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX)
            {
                Fire.position = Sub2;
                Fire.rotation = Rot2;
                spriteRenderer.flipX = true;
            }
            else if (Dir.x < 0 && spriteRenderer.flipX)
            {
                Fire.position = Sub1;
                Fire.rotation = Rot1;
                spriteRenderer.flipX = false;
            }

            if (!OnHit) rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime * (1 + GameManager.instance.EnemyStatus.speed));
        }
        if (BeginAttack && !anim.GetBool("IsAttack"))
        {
            AttackPos = Target.position;
            MoveAble = false;
            anim.SetBool("IsAttack", true);
        }
    }

    Vector3 Rot1 = new Vector3(0, 0, 30);
    Vector3 Rot2 = new Vector3(0, 0, -30);

    Vector3 Sub3 = new Vector3(-1.5f, 1, 0);
    Vector3 Sub4 = new Vector3(1.5f, 1, 0);
    protected override void AttackMethod()
    {
        Fire.position = spriteRenderer.flipX ? Sub4 : Sub3;
        Fire.rotation = spriteRenderer.flipX ? Rot1 : Rot2;
        base.AttackMethod();
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        Fire.position = spriteRenderer.flipX ? Sub2 : Sub1;
        Fire.rotation = spriteRenderer.flipX ? Rot2 : Rot1;
    }

    bool spec = false;
    protected override void HPChange()
    {
        if (spec) return;
        if(HP <= MaxHP * 0.5f)
        {
            FireSys.Play();
            MaxDamage *= 2; Damage = MaxDamage; BI.IgnoreDefense = 0.2f;
            spec = true;
        }
    }

    protected override IEnumerator DeadLater()
    {
        FireSys.Stop();
        MaxDamage = Mathf.FloorToInt(MaxDamage * 0.5f); spec = false; BI.IgnoreDefense = 0;
        return base.DeadLater();
    }
}
