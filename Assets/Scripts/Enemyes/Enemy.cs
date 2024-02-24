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
    WaitForSeconds WFS_Attack_AS;
    WaitForSeconds WFS_Attack_First;
    WaitForSeconds WFS_Attack_Last;

    protected virtual void Awake()
    {
        AttackType = null;
        rigid = GetComponent<Rigidbody2D>();
        WFS_Attack_AS = new WaitForSeconds(AS);
        WFS_Attack_First = new WaitForSeconds(FirstDelay);
        WFS_Attack_Last = new WaitForSeconds(LastDelay);
    }

    protected virtual void FixedUpdate()
    {
        rigid.velocity = Vector3.zero;
        if (MoveAble && !BeginAttack)
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && AttackType == null) AttackType = StartCoroutine(Attack()); 
    }

    protected virtual IEnumerator Attack()
    {
        while (BeginAttack)
        {
            yield return WFS_Attack_First;
            MoveAble = false;
            AttackMethod();
            yield return WFS_Attack_Last;
            MoveAble = true;
            if (!BeginAttack) break;
            yield return WFS_Attack_AS;
        }
        AttackType = null;
    }

    protected virtual void AttackMethod()
    {
        print("Attack!");
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.Self.transform.position * 1.8f - transform.position * 0.8f;
        }
    }

    protected virtual void OnDisable()
    {
        MoveAble = true;
        BeginAttack = false;
        StopAllCoroutines();
        AttackType = null;
    }
}
