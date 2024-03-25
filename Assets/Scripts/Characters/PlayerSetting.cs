using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviour
{
    [SerializeField] protected Player player;
    /*[SerializeField] protected Sprite WeaponIm;
    [SerializeField] protected Sprite HeadIm;*/
    
    public bool IsPlayer;
    public bool IsSummon = false;
    public bool HasWeapon;

    [NonSerialized] public bool CanMove = true;
    protected virtual void Awake()
    {
        IsPlayer = player.IsPlayer;
        player.Self = transform;
        player.rigid = GetComponent<Rigidbody2D>();
        player.anim = GetComponent<Animator>();
        player.sprite = GetComponent<SpriteRenderer>();
        player.WeaponLevel = 1;
        CanMove = IsPlayer;
        player.CurHP = player.InitHP; player.MaxHP = player.InitHP;
        //player.MaxSp = player.CurSP = player.InitSP;

        if (!IsSummon)
        {
            GameManager.instance.RequestOfWeapon(WeaponLevelUp, player.Id);
        }

        gameObject.SetActive(false);
    }


    protected virtual void FixedUpdate()
    {
        if (player.ChangeOccur)
        {
            player.ChangeOccur = false;
            int cnt = player.MaxHP;
            player.MaxHP = Mathf.FloorToInt(player.InitHP * (1 + player.HPRatio + GameManager.instance.PlayerStatus.hp));
            if (cnt - player.MaxHP != 0)
            {
                player.CurHP += cnt;
                HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
                if (!IsPlayer) player.MyBatch.HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
            }
            player.anim.SetFloat("AttackSpeed", player.AttackSpeed + GameManager.instance.PlayerStatus.attackspeed);
        }

        player.rigid.velocity = Vector2.zero;
        if (CanMove)
        {
            if (!IsPlayer)
            {
                if (player.IsFollow)
                {
                    TargetPos = GameManager.instance.Git.transform;
                    player.Dir = (TargetPos.position - transform.position).normalized;
                    if (Vector3.Distance(transform.position, TargetPos.position) <= 1.5f) player.IsFollow = false;
                }
                else
                {
                    TargetPos = GetNearest(scanRange);
                    if (TargetPos != null)
                    {
                        if (Vector3.Distance(transform.position, TargetPos.position) <= AttackRange) Attack();
                        player.Dir = (TargetPos.position - transform.position).normalized;
                    }
                    else player.Dir = Vector2.zero;
                }
            }
            Vector2 nextVec = player.Dir * player.speed * (1 + player.SpeedRatio + GameManager.instance.PlayerStatus.speed) *  Time.fixedDeltaTime;
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
        if (TargetPos != null && !player.IsFollow)
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

    [SerializeField] Image HPBar;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("EnemyAttack") && CanHit)
        {
            BulletInfo Info = GameManager.instance.BM.GetBulletInfo(GameManager.StringToInt(collision.name));
            int GetDamage = (int)(Info.Damage * (100 - player.InitDefense * (1 + player.DefenseRatio + GameManager.instance.PlayerStatus.defense)) * 0.01);
            player.CurHP -= GetDamage;
            if (player.CurHP > player.InitHP ) player.CurHP = player.MaxHP;
            else if (player.CurHP <= 0)
            {
                player.CurHP = 0;
                if (!IsPlayer)
                {
                    gameObject.SetActive(false);
                    player.MyBatch.ReBatch();
                }
            }

            HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
            if (IsPlayer) GameManager.instance.UM.HpChange();
            else if(!IsSummon) {  player.MyBatch.HPBar.fillAmount = player.CurHP / (float)player.MaxHP;  }
            if(player.CurHP > 0 && GetDamage > 0) StartCoroutine(NockBack_Player());
        }
        else if (collision.CompareTag("PlayerBuff"))
        {
            Buff Info = GameManager.instance.BM.GetBulletInfo(GameManager.StringToInt(collision.name)).Buffs;
            if (Info.Heal != 0) Heal(Info.Heal);
        }
    }
    protected void Heal(int Amount)
    {
        Amount = (int)(Amount * GameManager.instance.PlayerStatus.heal);
        int LeftHP = player.MaxHP - player.CurHP;
        if (Amount > LeftHP) Amount = LeftHP;
        if (LeftHP != 0)
        {
            player.CurHP += Amount;
            GameManager.instance.DM.MakeHealCount(Amount, transform);
            HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
            if (IsPlayer) GameManager.instance.UM.HpChange();
            else if (!IsSummon) { player.MyBatch.HPBar.fillAmount = player.CurHP / (float)player.MaxHP; }
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

    protected virtual void OnEnable()
    {
        player.CurHP = player.MaxHP;
        HPBar.fillAmount = 1;
        player.AttackSpeed = player.MaxAttackSpeed;
        player.anim.SetFloat("AttackSpeed", player.MaxAttackSpeed + GameManager.instance.PlayerStatus.attackspeed);

        if (!IsPlayer)
        {
            player.anim.SetBool("IsAttack", false);
            if (!IsSummon)
            {
                player.MyBatch.HPBar.fillAmount = 1;
            }
        }
    }
}
