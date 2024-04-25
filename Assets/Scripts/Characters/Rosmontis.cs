using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rosmontis : PlayerSetting
{
    [SerializeField] GameObject[] SubWeapons;
    [SerializeField] Sprite[] Sprites;


    [SerializeField]
    Sprite BigBullet;

    Vector3[] WeaponOrigin;
    
    SpriteRenderer[] WeaponSprite;
    TrailRenderer[] WeaponTrail;

    List<Transform> Targets;

    bool IsDouble = true;

    protected override void Awake()
    {
        WeaponSprite = new SpriteRenderer[2];
        WeaponTrail = new TrailRenderer[2];
        
        WeaponSprite[0] = SubWeapons[0].GetComponent<SpriteRenderer>(); WeaponTrail[0] = SubWeapons[0].GetComponent<TrailRenderer>();
        WeaponSprite[1] = SubWeapons[1].GetComponent<SpriteRenderer>(); WeaponTrail[1] = SubWeapons[1].GetComponent<TrailRenderer>();
        WeaponTrail[0].enabled = WeaponTrail[1].enabled = false;
        
        base.Awake();
    }


    protected override void FixedUpdate()
    {
        if (player.ChangeOccur)
        {
            player.ChangeOccur = false;
            int cnt = player.MaxHP;
            player.MaxHP = Mathf.FloorToInt(player.InitHP * (1 + player.HPRatio + GameManager.instance.PlayerStatus.hp));
            if (cnt - player.MaxHP != 0)
            {
                player.CurHP += player.MaxHP - cnt;
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
                else if(!IsDouble)
                {
                    TargetPos = GetNearest(scanRange);
                    if (TargetPos != null)
                    {
                        if (Vector3.Distance(transform.position, TargetPos.position) <= AttackRange) Attack();
                        player.Dir = (TargetPos.position - transform.position).normalized;
                    }
                    else player.Dir = Vector2.zero;
                }
                else
                {
                    Targets = GameManager.GetNearest(scanRange, 2, transform.position, targetLayer);
                    if (Targets.Count != 0)
                    {
                        if (Vector3.Distance(transform.position, Targets[0].position) <= AttackRange) Attack();
                        player.Dir = (Targets[0].position - transform.position).normalized;
                    }
                    else player.Dir = Vector2.zero;
                }
            }
            Vector2 nextVec = player.Dir * player.speed * (1 + player.SpeedRatio + GameManager.instance.PlayerStatus.speed) * Time.fixedDeltaTime;
            if (nextVec.Equals(Vector2.zero))
            {
                player.anim.SetBool("IsWalk", false);
            }
            else
            {
                if (player.Dir.x > 0 && !player.sprite.flipX)
                {
                    player.sprite.flipX = true;
                    foreach (var k in player.SubEffects) k.flipX = true;
                }
                else if (player.Dir.x < 0 && player.sprite.flipX)
                {
                    player.sprite.flipX = false;
                    foreach (var k in player.SubEffects) k.flipX = false;
                }
                player.anim.SetBool("IsWalk", true);
                player.rigid.MovePosition(player.rigid.position + nextVec);
            }
            WeaponAnim();
        }
    }

    protected override void EndBatch()
    {
        CanMove = true;
        SubWeapons[0].SetActive(true); SubWeapons[1].SetActive(true);
    }

    bool OneLast = false;


    void Attack1()
    {
        StartCoroutine(AttackSub());
    }

    IEnumerator AttackSub()
    {
        if (!OneLast)
        {
            WeaponSprite[0].sprite = Sprites[0];
            WeaponTrail[0].Clear();
            WeaponTrail[0].enabled = true;
            for (int i = 0; i < 5; i++)
            {
                SubWeapons[0].transform.Translate(Vector2.up * 2);
                yield return new WaitForSeconds(0.05f);
            }
            SubWeapons[0].SetActive(false);
        }
        else
        {
            WeaponSprite[1].sprite = Sprites[1];
            WeaponTrail[1].Clear();
            WeaponTrail[1].enabled = true;
            for (int i = 0; i < 5; i++)
            {
                SubWeapons[1].transform.Translate(Vector2.up * 2);
                yield return new WaitForSeconds(0.05f);
            }
            SubWeapons[1].SetActive(false);
        }
        if (IsDouble) {
            OneLast = !OneLast;
            if (OneLast) { yield return new WaitForSeconds(0.1f); StartCoroutine(AttackSub()); }
        }
    }

    protected override void AttackMethod()
    {
        StartCoroutine(AttackSub2());
    }

    void Attack2()
    {
        StartCoroutine(AttackSub());
    }

    IEnumerator AttackSub2()
    {
        Vector3 AttackPos;
        if (!OneLast)
        {
            SubWeapons[0].SetActive(true);
            if (!IsDouble) AttackPos = TargetPos.position; else AttackPos = Targets[0].position;
            SubWeapons[0].transform.position = AttackPos + Vector3.up * 10f;
            WeaponTrail[0].Clear();
            for (int i = 0; i < 5; i++)
            {
                SubWeapons[0].transform.Translate(Vector2.down * 2);
                yield return new WaitForSeconds(0.05f);
            }
            WeaponSprite[0].sprite = Sprites[2];
        }
        else
        {
            SubWeapons[1].SetActive(true);
            if (!IsDouble) AttackPos = TargetPos.position; else if (Targets.Count == 2) AttackPos = Targets[1].position; else AttackPos = Targets[0].position;
            SubWeapons[1].transform.position = AttackPos + Vector3.up * 10f;
            WeaponTrail[1].Clear();
            for (int i = 0; i < 5; i++)
            {
                SubWeapons[1].transform.Translate(Vector2.down * 2);
                yield return new WaitForSeconds(0.05f);
            }
            WeaponSprite[1].sprite = Sprites[3];
        }

        int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);

        GameManager.instance.BM.MakeMeele
            (
            new BulletInfo(Damage,false,0,debuffs: new DeBuff(5, defense: defenseRatio)), 0.3f,
                    AttackPos, Vector3.zero, 0, false,Sprites[4]);

        if (IsDouble)
        {
            OneLast = !OneLast;
            if (OneLast) { yield return new WaitForSeconds(0.1f); StartCoroutine(AttackSub2()); }
        }
    }

    protected override void AttackEnd()
    {
        if (!SubWeapons[0].activeSelf)
        {
            WeaponTrail[0].enabled = false; SubWeapons[0].SetActive(true); SubWeapons[0].transform.localPosition = Vector3.zero;
            OneLast = true;
        }
        if (!SubWeapons[1].activeSelf)
        {
            WeaponTrail[1].enabled = false; SubWeapons[1].SetActive(true); SubWeapons[1].transform.localPosition = Vector3.zero;
            OneLast = false;
        }
        base.AttackEnd();
    }

    float DamageRatio = 7.5f;
    float defenseRatio = 0.3f;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio ++; break;
            case 2: Sprites[4] = BigBullet; break;
            case 3: player.AttackSpeed *= 1.2f; player.anim.SetFloat("AttackSpeed", player.AttackSpeed); break;
            case 4: defenseRatio += 0.1f; break;
            case 5: defenseRatio += 0.1f; break;
            case 6: IsDouble = true; OneLast = false; break;
        }
        return player.WeaponLevel;
    }
}
