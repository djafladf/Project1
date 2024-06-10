using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected int HP;
    [SerializeField] protected float speed;
    [SerializeField] protected int Damage;
    [SerializeField] protected int Defense;
    [SerializeField] protected int Weight;
    [SerializeField] protected int ItemVal;
    protected int MaxDefense;
    protected int MaxDamage;
    protected float MaxSpeed;

    protected Rigidbody2D rigid;


    [NonSerialized] public bool MoveAble = true;
    [NonSerialized] public bool BeginAttack = false;
    [NonSerialized] public Transform Target = null;

    protected int MaxHP;
    protected bool IsLive = true;
    protected bool OnIce = false;
    protected bool OnHit = false;

    protected float IceRatio = 1;
    protected float Cheeled = 0;

    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D coll;

    protected virtual void Awake()
    {
        MaxHP = HP; MaxDamage = Damage; MaxDefense = Defense; MaxSpeed = speed;
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (!IsLive || OnIce) return;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            if (!OnHit) rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime * (1 + GameManager.instance.EnemyStatus.speed));
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
        else AttackPos = Target.position;
    }

    protected Vector3 AttackPos;


    //[SerializeField] protected Sprite Bullet;
    [SerializeField] bool IsRange;
    [SerializeField] bool MakeLine;
    [SerializeField] protected BulletInfo BI;
    [SerializeField] protected Sprite Bull;
    [SerializeField] BulletLine BL;

    protected virtual void AttackMethod()
    {

        if (IsRange)
        {
            BI.Damage = Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack));
            GameManager.instance.BM.MakeBullet(BI, 0, transform.position, (AttackPos - transform.position).normalized, 8, true, Bull, BL: MakeLine ? BL : null);
        }
        else
        {
            int CurDm = Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack));
            GameManager.instance.BM.MakeMeele(new BulletInfo(CurDm, false, 0), 0.2f, AttackPos, Vector2.zero, 0, true);
        }
    }

    bool CanHit = true;
    float[] LeftTime = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    float[] DeBuffVar = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

    protected virtual void HPChange()
    {
        
    }

    protected virtual IEnumerator DeadLater()
    {
        yield return GameManager.DotOneSec;
        tag = "Untagged";
        StopAllCoroutines();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("PlayerAttack") && CanHit)
        {
            BulletInfo Info = GameManager.instance.BM.GetBulletInfo(GameManager.StringToInt(collision.name));
            int GetDamage = Info.ReturnDamage(Defense * (1 + GameManager.instance.EnemyStatus.defense));
            GameManager.instance.DM.MakeDamage(GetDamage, transform);
            HP -= GetDamage;
            HPChange();     // For Boss
            if (HP <= 0)
            {
                anim.SetTrigger("Dead");
                StartCoroutine(DeadLater());
                spriteRenderer.sortingOrder = 1;
                IsLive = false; CanHit = false; rigid.simulated = false; coll.enabled = false;
                GameManager.instance.UM.KillCountUp(1); GameManager.instance.ES.CurActive--;
            }
            else if (Info.DeBuffs != null)
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
                if (Info.DeBuffs.Ice != 0 && !OnIce)
                {
                    Cheeled += Info.DeBuffs.Ice * IceRatio;
                    if (DeBuffObj[3] == null)
                    {
                        DeBuffObj[3] = GameManager.instance.BFM.RequestForDebuff(0);
                        DeBuffObj[3].transform.parent = transform;
                        DeBuffObj[3].transform.localPosition = new Vector3(0, spriteRenderer.sprite.bounds.size.y * 0.6f, 0);
                        DeBuffObj[3].gameObject.SetActive(true);
                        speed *= 0.7f;
                    }
                    if (Cheeled >= 5)
                    {
                        DeBuffObj[3].SetActive(false); DeBuffObj[3].transform.parent = GameManager.instance.BFM.transform;
                        DeBuffObj[3] = null;

                        DeBuffObj[3] = GameManager.instance.BFM.RequestForDebuff(1,spriteRenderer.sprite.bounds.size.x,spriteRenderer.bounds.size.y);
                        DeBuffObj[3].transform.parent = transform;
                        DeBuffObj[3].transform.localPosition = Vector3.zero;
                        DeBuffObj[3].gameObject.SetActive(true);
                        StartCoroutine(Iced());
                    }
                }
                if (Info.DeBuffs.Fragility != 0)
                {

                }
            }
            StartCoroutine(NockBack_Enemy(Info.KnockBack,transform.position - collision.transform.position));
        }
    }
    protected GameObject[] DeBuffObj = new GameObject[5];

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

    IEnumerator Iced()
    {
        anim.enabled = false;
        OnIce = true;
        yield return GameManager.OneSec;

        Cheeled = 0;
        anim.enabled = true;
        IceRatio *= 0.5f;
        OnIce = false;
        DeBuffObj[3].SetActive(false); DeBuffObj[3].transform.parent = GameManager.instance.BFM.transform;
        DeBuffObj[3] = null;
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsLive) return;
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.Self.position + GameManager.instance.ES.ReBatchCall();
        }
    }

    Coroutine Fric = null;

    IEnumerator NockBack_Enemy(float Power,Vector2 Dir)
    {
        CanHit = false; 
        spriteRenderer.color = Color.gray;
        Power += GameManager.instance.PlayerStatus.power - Weight * 5;
        OnHit = Power > 0;
        if (Power > 0) 
        {
            rigid.AddForce(Dir.normalized * (Power), ForceMode2D.Impulse);
            if (Fric == null) Fric = StartCoroutine(Friction());
        }
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        CanHit = true;
    }

    IEnumerator Friction()
    {
        while (rigid.velocity.magnitude >= 0.2f)
        {
            rigid.velocity = Vector2.Lerp(rigid.velocity, Vector2.zero, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
        rigid.velocity = Vector2.zero;
        Fric = null; OnHit = false;
    }


    protected virtual void Dead()
    {
        GameManager.instance.IM.MakeItem(transform.position);
        gameObject.SetActive(false);
    }
    bool IsInit = true;
    protected virtual void OnEnable()
    {
        if (IsInit) { IsInit = false; return; }
        for (int i = 0; i < DeBuffObj.Length; i++)
        {
            if (DeBuffObj[i] != null)
            {
                LeftTime[i] = 0;
                DeBuffObj[i].SetActive(false); DeBuffObj[i].transform.parent = GameManager.instance.BFM.transform; DeBuffObj[i] = null;
            }
        }
        MoveAble = true; OnHit = false; Fric = null;
        
        anim.enabled = true;
        anim.SetBool("IsAttack", false);
        BeginAttack = false;
        CanHit = true;
        rigid.simulated = true;
        coll.enabled = true;
        HP = Mathf.FloorToInt(MaxHP * (1 + GameManager.instance.EnemyStatus.hp));

        IsLive = true;

        OnIce = false; IceRatio = 1; Cheeled = 0;
        Defense = MaxDefense; speed = MaxSpeed; Damage = MaxDamage;

        spriteRenderer.color = Color.white;
        spriteRenderer.sortingOrder = 2;

        tag = "Enemy";
    }
}
