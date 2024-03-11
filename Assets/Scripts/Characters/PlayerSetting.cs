using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerSetting : MonoBehaviour
{
    [SerializeField] protected Player player;
    [SerializeField] protected Sprite WeaponIm;
    [SerializeField] protected Sprite HeadIm;
    [SerializeField] protected GameObject Weapon;
    public bool IsPlayer;
    public bool IsSummon = false;
    public bool HasWeapon;

    [NonSerialized] public bool CanMove = true;
    protected virtual void Awake()
    {
        if (HasWeapon) 
        { 
            player.Weapon = Weapon; player.WeaponPos = player.Weapon.transform.localPosition; player.FlipWeaponPos = new Vector3(-player.WeaponPos.x, player.WeaponPos.y);
        }
        IsPlayer = player.IsPlayer;
        player.Self = transform;
        player.rigid = GetComponent<Rigidbody2D>();
        player.anim = GetComponent<Animator>();
        player.sprite = GetComponent<SpriteRenderer>();
        player.WeaponLevel = 1;
        CanMove = IsPlayer;

        if(!IsSummon) GameManager.instance.RequestOfWeapon(WeaponLevelUp, player.Id);

        AttackInf = new AttackType();
        gameObject.SetActive(false);
    }


    protected virtual void FixedUpdate()
    {
        player.rigid.velocity = Vector2.zero;
        if (CanMove)
        {
            if (!IsPlayer)
            {
                TargetPos = GetNearest(scanRange);
                if (TargetPos != null)
                {
                    if (Vector3.Distance(transform.position, TargetPos.position) <= AttackRange) Attack();
                    player.Dir = (TargetPos.position - transform.position).normalized;  
                }
                else player.Dir = Vector2.zero;
            }
            Vector2 nextVec = player.Dir * player.speed * Time.fixedDeltaTime;
            if (nextVec.Equals(Vector2.zero))
            {
                player.anim.SetBool("IsWalk", false);
            }
            else
            {
                if (player.Dir.x > 0 && !player.sprite.flipX)
                {
                    player.sprite.flipX = true;
                    if (HasWeapon) player.Weapon.transform.localPosition = player.FlipWeaponPos;
                }
                else if (player.Dir.x < 0 && player.sprite.flipX)
                {
                    player.sprite.flipX = false;
                    if (HasWeapon) player.Weapon.transform.localPosition = player.WeaponPos;
                }
                player.anim.SetBool("IsWalk", true);
                player.rigid.MovePosition(player.rigid.position + nextVec);
            }
            WeaponAnim();
        } 
    }

    protected virtual void OnMove(InputValue value)
    {
        player.Dir = value.Get<Vector2>();
    }
    protected virtual void WeaponAnim()
    {

    }
    protected virtual int WeaponLevelUp()
    {
        return -1;
    }


    // About Assistant ----------------------------------------------

    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected float scanRange;
    protected Transform TargetPos = null;

    Transform GetNearest(float Range)
    {
        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, Range, Vector2.zero, 0, targetLayer);
        float diffs = scanRange + 10;
        Transform res = null;
        foreach(RaycastHit2D target in targets)
        {
            float curDiff = Vector3.Distance(transform.position, target.transform.position);
            if(curDiff < diffs)
            {
                diffs = curDiff; res = target.transform;
            }
        }

        return res;
    }

    protected AttackType AttackInf;
    [SerializeField] protected float AttackRange;
    protected Transform AttackTarget = null;

    protected virtual void Attack()
    {
        player.anim.SetBool("IsAttack", true);
        CanMove = false;
    }

    protected virtual void AttackEnd()
    {
        TargetPos = GetNearest(scanRange);
        if (TargetPos != null)
        {
            if (Vector3.Distance(transform.position, TargetPos.position) > AttackRange)
            {
                player.anim.SetBool("IsAttack", false);
                CanMove = true;
            }
            else
            {
                player.Dir = (TargetPos.position - transform.position).normalized;
                if (player.Dir.x > 0 && !player.sprite.flipX)
                {
                    player.sprite.flipX = true;
                    if (HasWeapon) player.Weapon.transform.localPosition = player.FlipWeaponPos;
                }
                else if (player.Dir.x < 0 && player.sprite.flipX)
                {
                    player.sprite.flipX = false;
                    if (HasWeapon) player.Weapon.transform.localPosition = player.WeaponPos;
                }
            }
        }
        else
        {
            player.anim.SetBool("IsAttack", false);
            CanMove = true;
        }
    }

    protected virtual void AttackMethod()
    {

    }

    protected virtual void EndBatch()
    {
        CanMove = true;
    }

    bool CanHit = true;

    [SerializeField] Transform HPBar;
    [SerializeField] Transform SPBar;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack") && CanHit)
        {
            
            int GetDamage = int.Parse(collision.name);
            player.CurHP -= GetDamage;
            if (player.CurHP > player.MaxHP) player.CurHP = player.MaxHP;
            else if (player.CurHP <= 0)
            {
                player.CurHP = 0;
                if (!IsPlayer)
                {
                    gameObject.SetActive(false);
                    player.MyBatch.ReBatch();
                }
            }

            HPBar.localScale -= Vector3.right * GetDamage / player.MaxHP;
            if (HPBar.localScale.x > 1) HPBar.localScale = Vector3.one;
            if (IsPlayer) GameManager.instance.HpChange();
            else {  player.MyBatch.HPBar.fillAmount = (float)player.CurHP / (float)player.MaxHP;  }
            if(player.CurHP > 0 && GetDamage > 0) StartCoroutine(NockBack_Player());
        }
    }

    IEnumerator NockBack_Player()
    {
        CanHit = false;
        player.sprite.color = Color.gray;
        yield return new WaitForSeconds(0.2f);
        player.sprite.color = Color.white;
        CanHit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area"))
        {
            transform.position = GameManager.instance.player.Self.transform.position * 1.5f - transform.position * 0.5f;
        }
    }

    private void OnEnable()
    {
        player.CurHP = player.MaxHP;
        player.CurSP = player.MaxSP;
        HPBar.localScale = Vector3.one;
        SPBar.localScale = Vector3.up;
        if (!IsPlayer)
        {
            player.anim.SetBool("IsAttack", false);
            if (!IsSummon)
            {
                player.MyBatch.HPBar.fillAmount = 1;
                player.MyBatch.SPBar.fillAmount = 0;
            }
        }
    }
}
