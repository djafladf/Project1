using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rigid;
    CapsuleCollider2D coll;
    SpriteRenderer sprite;
    Animator anim;
    Sprite HitImage;
    TrailRenderer Line;


    BulletInfo AfterBull;

    int Penetrate;
    
    bool IsMeele;
    bool IsEnem;
    bool IsBoom;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>(); anim.enabled = false;
        Line = GetComponent<TrailRenderer>(); Line.enabled = false;
    }

    public void Init_Attack(int Penetrate, Vector3 Dir,
        bool IsMeele, bool IsEnemy, float AfterTime, Sprite Image = null, 
        BulletLine BL = null, RuntimeAnimatorController Anim = null,Sprite HitImage = null)
    {
        if (BL != null)
        {
            Line.enabled = true;
            Line.startColor = BL.Start; Line.endColor = BL.End;
            Line.startWidth = BL.StartWidth; Line.endWidth = BL.EndWidth;
            Line.time = BL.Time;
        }
        if (Anim != null) { anim.enabled = true; anim.runtimeAnimatorController = Anim; }

        this.HitImage = HitImage;
        rigid.simulated = true; coll.enabled = true;rigid.velocity = Dir; this.Penetrate = Penetrate; sprite.sprite = Image; this.IsMeele = IsMeele; this.IsEnem = IsEnemy; IsBoom = false;
        
        if (Image != null) coll.size = sprite.bounds.size * 0.9f;
        else coll.size = Vector2.one;
        
        if (IsMeele) StartCoroutine(AfterImage(AfterTime));
        
        tag = IsEnemy ? "EnemyAttack" : "PlayerAttack";
    }

    public void Init_Explode(BulletInfo After, Vector3 Dir, bool IsEnemy, Sprite Image, Sprite HitImage,
        BulletLine BL = null, RuntimeAnimatorController Anim = null)
    {
        if (BL != null)
        {
            Line.enabled = true;
            Line.startColor = BL.Start; Line.endColor = BL.End;
            Line.startWidth = BL.StartWidth; Line.endWidth = BL.EndWidth;
            Line.time = BL.Time;
        }
        if (Anim != null) { anim.enabled = true; anim.runtimeAnimatorController = Anim; }

        this.HitImage = HitImage; this.AfterBull = After;
        rigid.simulated = true; coll.enabled = true; rigid.velocity = Dir; this.Penetrate = 0; sprite.sprite = Image; this.IsMeele = false; this.IsEnem = IsEnemy; IsBoom = true;

        if (Image != null) coll.size = sprite.bounds.size * 0.9f;
        else coll.size = Vector2.one;

        tag = IsEnemy ? "EnemyAttack" : "PlayerAttack";
    }


    public void Init_Effect(float AfterTime, Sprite Image,Vector3 Dir, BulletLine BL = null,RuntimeAnimatorController Anim = null)
    {
        if (BL != null)
        {
            Line.enabled = true;
            Line.startColor = BL.Start; Line.endColor = BL.End;
            Line.startWidth = BL.StartWidth; Line.endWidth = BL.EndWidth;
            Line.time = BL.Time;
        }
        if (Anim != null){ anim.enabled = true; anim.runtimeAnimatorController = Anim; }

        rigid.simulated = true; rigid.velocity = Dir; sprite.sprite = Image;
        if (BL == null) Line.enabled = false;

        StartCoroutine(AfterImage(AfterTime));
    }

    public void Init_Buff(Sprite Im,  bool IsEnemy)
    {
        rigid.simulated = true;
        coll.enabled = true;
        sprite.sprite = Im;
        IsMeele = false;
        tag = IsEnemy ? "EnemyBuff" : "PlayerBuff";
    }

    public void OnDisable()
    {
        Line.Clear(); Line.enabled = false;
        anim.enabled = false; rigid.simulated = false; coll.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsMeele) { coll.enabled = false; return; }
        if (collision.CompareTag("Enemy") && !IsEnem)
        {
            if (HitImage != null)
            {
                if (IsBoom) GameManager.instance.BM.MakeMeele(AfterBull, 0.3f, transform.position, Vector3.zero,0, true, HitImage);
                else GameManager.instance.BM.MakeEffect(0.3f, transform.position, Vector3.zero,0, HitImage);
            }
            if (Penetrate-- <= 0) gameObject.SetActive(false);
        }
        else if((collision.CompareTag("Player")||collision.CompareTag("Player_Hide")) && IsEnem)
        {
            if (HitImage != null)
            {
                if (IsBoom) GameManager.instance.BM.MakeMeele(AfterBull, 0.3f, transform.position, Vector3.zero, 0,true, HitImage);
                else GameManager.instance.BM.MakeEffect(0.3f, transform.position, Vector3.zero, 0,HitImage);
            }
            if (Penetrate-- <= 0) gameObject.SetActive(false);
        }

    }

    IEnumerator AfterImage(float AfterTime) 
    {
        yield return new WaitForSeconds(AfterTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area") && !IsMeele) gameObject.SetActive(false);
    }
}
