using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float AS;
    [SerializeField] float FirstDelay;
    [SerializeField] float LastDelay;
    Rigidbody2D rigid;
    [NonSerialized] public bool MoveAble = true;
    [NonSerialized] public bool BeginAttack = false;

    Coroutine AttackType;

    int MaxHP;
    bool IsLive = true;

    WaitForSeconds WFS_Attack_AS;
    WaitForSeconds WFS_Attack_First;
    WaitForSeconds WFS_Attack_Last;

    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D coll;

    protected virtual void Awake()
    {
        MaxHP = HP;
        AttackType = null;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        WFS_Attack_AS = new WaitForSeconds(AS);
        WFS_Attack_First = new WaitForSeconds(FirstDelay);
        WFS_Attack_Last = new WaitForSeconds(LastDelay);
        coll = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsLive) return;
        rigid.velocity = Vector2.zero;
        if (MoveAble && !BeginAttack)
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && AttackType == null) AttackType = StartCoroutine(Attack()); 
    }

    protected virtual IEnumerator Attack()
    {
        while (BeginAttack)
        {
            anim.SetBool("IsAttack", true);
            yield return WFS_Attack_First;
            MoveAble = false;
            AttackMethod();
            yield return WFS_Attack_Last;
            anim.SetBool("IsAttack", false);
            MoveAble = true;
            if (!BeginAttack) break;
            yield return WFS_Attack_AS;
        }
        AttackType = null;
    }

    protected virtual void AttackMethod()
    {
        if (BeginAttack) GameManager.instance.HpChange(-1);
    }

    bool CanHit = true;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("PlayerAttack") && CanHit)
        {
            HP--;
            if (HP <= 0)
            {
                anim.SetTrigger("Dead");
                spriteRenderer.sortingOrder = 2;
                IsLive = false;
                CanHit = false;
                GameManager.instance.KillCountUp(1);
                transform.position = rigid.position;
                rigid.simulated = false;
                coll.enabled = false;
            }
            else StartCoroutine(NockBack_Enemy());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.Self.transform.position * 1.8f - transform.position * 0.8f;
        }
    }


    IEnumerator NockBack_Enemy()
    {
        CanHit = false;
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
        CanHit = true;
    }
    void Dead()
    {
        GameManager.instance.IM.MakeItem(transform);
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual void OnEnable()
    {
        MoveAble = true;
        BeginAttack = false;
        AttackType = null;
        CanHit = true;
        rigid.simulated = true;
        coll.enabled = true;
        HP = MaxHP;
        IsLive = true;
        spriteRenderer.sortingOrder = 5;
    }
}
