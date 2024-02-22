using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float AS;
    Rigidbody2D rigid;
    [NonSerialized] public bool MoveAble = true;
    [NonSerialized] public bool BeginAttack = false;

    Coroutine AttackType;
    WaitForSeconds WFS_Attack;

    protected virtual void Awake()
    {
        AttackType = null;
        rigid = GetComponent<Rigidbody2D>();
        WFS_Attack = new WaitForSeconds(AS);
    }

    protected virtual void FixedUpdate()
    {
        rigid.velocity = Vector3.zero;
        if (MoveAble && !BeginAttack)
        {
            Vector2 Dir = (GameManager.instance.player.transform.position - transform.position).normalized;
            rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && AttackType == null) AttackType = StartCoroutine(Attack()); 
    }

    protected virtual IEnumerator Attack()
    {
        while (BeginAttack)
        {
            AttackMethod();
            yield return WFS_Attack;
        }
        AttackType = null;
    }

    protected virtual void AttackMethod()
    {

    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.transform.position * 1.8f - transform.position * 0.8f;
            print("RePos!");
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
