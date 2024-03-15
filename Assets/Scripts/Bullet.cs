using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
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

    public void Init_Attack(int Damage,int Penetrate, Vector3 Dir,
        bool IsMeele,bool IsEnemy,float AfterTime, Sprite Image)
    {
        rigid.simulated = true;
        coll.enabled = true;
        rigid.velocity = Dir; this.Penetrate = Penetrate; sprite.sprite = Image;
        this.IsMeele = IsMeele; this.AfterTime = AfterTime;
        if (Image != null) coll.size = sprite.bounds.size * 0.9f;
        else coll.size = Vector2.one;
        
        if (IsMeele) StartCoroutine(AfterImage());
        tag = IsEnemy ? "EnemyAttack" : "PlayerAttack";
    }

    public void Init_Effect(float AfterTime, Sprite Image)
    {
        rigid.simulated = false;
        coll.enabled = false;
        sprite.sprite = Image;
        IsMeele = true;
        StartCoroutine(AfterImage());
    }

    public void Init_Buff(Sprite Im,  bool IsEnemy)
    {
        rigid.simulated = true;
        coll.enabled = true;
        sprite.sprite = Im;
        IsMeele = false;
        tag = IsEnemy ? "EnemyBuff" : "PlayerBuff";
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
