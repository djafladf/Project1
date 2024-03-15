using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float AS;
    [SerializeField] int Damage;
    [SerializeField] int Defense;
    int MaxDefense;
    int MaxDamage;

    Rigidbody2D rigid;


    [NonSerialized] public bool MoveAble = true;
    [NonSerialized] public bool BeginAttack = false;
    [NonSerialized] public Transform Target = null;

    int MaxHP;
    bool IsLive = true;

    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D coll;

    protected virtual void Awake()
    {
        MaxHP = HP;
        MaxDamage = Damage;
        MaxDefense = Defense;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsLive) return;
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
            anim.SetBool("IsAttack", true);
        }
    }

    protected virtual void AttackEnd()
    {
        if (!BeginAttack)
        {
            anim.SetBool("IsAttack", false);
            MoveAble = true;
        }
    }

    Vector3 AttackPos;

    protected virtual void AttackMethod()
    {
        GameManager.instance.BM.MakeMeele(Damage, 0, 0.2f, AttackPos, Vector2.zero, 0, null, true);
    }

    bool CanHit = true;
    float[] LeftTime = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    float[] DeBuffVar = { 1,1,1,1,1,1,1,1,1,1};

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("PlayerAttack") && CanHit)
        {
            BulletInfo Info = GameManager.instance.BM.GetBulletInfo(GameManager.StringToInt(collision.name));
            int GetDamage = (int)(Info.Damage * (100 - Defense) * 0.01);
            GameManager.instance.DM.MakeDamage(GetDamage,transform);
            HP -= GetDamage;
            if (HP <= 0)
            {
                anim.SetTrigger("Dead");
                spriteRenderer.sortingOrder = 1;
                IsLive = false; CanHit = false; rigid.simulated = false; coll.enabled = false;
                GameManager.instance.UM.KillCountUp(1); GameManager.instance.ES.CurActive--;
            }
            else
            {
                if(Info.DeBuffs != null)
                {
                    if (Info.DeBuffs.Speed != 1)
                    {

                    }
                    if (Info.DeBuffs.Attack != 1)
                    {

                    }
                    if (Info.DeBuffs.Defense != 1)
                    {
                        if (DeBuffVar[2] > Info.DeBuffs.Defense)
                        {
                            Defense = (int)(MaxDefense * Info.DeBuffs.Defense);
                            DeBuffVar[2] = Info.DeBuffs.Defense;
                        }

                        if (LeftTime[2] < Info.DeBuffs.Last)
                        {
                            if (LeftTime[2] <= 0)
                            {
                                DeBuffObj[2] = GameManager.instance.BFM.RequestForDebuff(3);
                                DeBuffObj[2].transform.parent = transform;
                                DeBuffObj[2].transform.localPosition = spriteRenderer.sprite.bounds.size * 0.5f;
                                DeBuffObj[2].gameObject.SetActive(true);
                                LeftTime[2] = Info.DeBuffs.Last;
                                StartCoroutine(DefenseChange());
                            }
                            else LeftTime[2] = Info.DeBuffs.Last;
                        }

                    }
                    if (Info.DeBuffs.Ice != 1)
                    {

                    }
                    if (Info.DeBuffs.Fragility != 1)
                    {

                    }
                }
                StartCoroutine(NockBack_Enemy());
            }
        }
    }
    GameObject[] DeBuffObj = new GameObject[5]; 

    IEnumerator DefenseChange()
    {
        while (LeftTime[2] > 0)
        {
            yield return GameManager.OneSec;
            LeftTime[2]--;
        }
        Defense = MaxDefense;
        DeBuffObj[2].SetActive(false); DeBuffObj[2].transform.parent = GameManager.instance.BFM.transform;
        DeBuffObj[2] = null;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.Self.position + GameManager.instance.ES.ReBatchCall();
        }
    }


    IEnumerator NockBack_Enemy()
    {
        CanHit = false;
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        CanHit = true;
    }
    void Dead()
    {
        GameManager.instance.IM.MakeItem(transform);
        gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        for(int i = 0; i < DeBuffObj.Length;i++)
        {
            if (DeBuffObj[i] != null)
            {
                LeftTime[i] = 0;
                DeBuffObj[i].SetActive(false); DeBuffObj[i].transform.parent = GameManager.instance.BFM.transform; DeBuffObj[i] = null;
            }
        }
        MoveAble = true;
        anim.SetBool("IsAttack", false);
        BeginAttack = false;
        CanHit = true;
        rigid.simulated = true;
        coll.enabled = true;
        HP = MaxHP;
        Defense = MaxDefense;
        IsLive = true;
        spriteRenderer.sortingOrder = 2;

        StopAllCoroutines();
    }
}
