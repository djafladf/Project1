using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [NonSerialized] public int Damage;
    [NonSerialized] public float KnockBackPower;

    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    SpriteRenderer sprite;
    int Penetrate;
    float AfterTime;
    bool IsMeele;

    Coroutine Meele;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    public void Init(int Damage,int Penetrate, Vector3 Dir,bool IsMeele,bool IsEnemy,float AfterTime)
    {
        rigid.velocity = Dir;
        this.Penetrate = Penetrate;
        name = $"{Damage}";
        if (sprite.sprite != null) coll.size = sprite.bounds.size * 0.9f;
        else coll.size = Vector2.one;
        this.IsMeele = IsMeele;
        this.AfterTime = AfterTime;
        if (IsMeele) StartCoroutine(AfterImage());
        tag = IsEnemy ? "EnemyAttack" : "PlayerAttack";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !IsMeele)
        {
            if (Penetrate > 0) Penetrate--;
            else gameObject.SetActive(false);
        }

    }

    IEnumerator AfterImage() 
    {
        yield return new WaitForSeconds(AfterTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area")) gameObject.SetActive(false);
    }
}
