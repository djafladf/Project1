using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Sora : PlayerSetting
{
    [SerializeField] ParticleSystem Norm;
    [SerializeField] ParticleSystem Spec;
    [SerializeField] List<Image> Synthe;
    [SerializeField] Transform ForceField;
    [SerializeField] AfterImMaker AIM_Force;

    [SerializeField] GameObject FlyOne;
    [SerializeField] GameObject FlyTwo;

    [SerializeField] List<Sprite> ETC;


    bool SpecO = false;

    int MaxScale = 20;

    protected override void Awake()
    {
        base.Awake();
        player.SubEffects.Add(transform.GetChild(1).GetComponent<SpriteRenderer>());
        player.SubEffects.Add(FlyOne.GetComponent<SpriteRenderer>());
        player.SubEffects.Add(FlyTwo.GetComponent<SpriteRenderer>());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var k in Synthe) k.gameObject.SetActive(false);
    }

    new protected void FixedUpdate()
    {
        if (player.ChangeOccur && !IsSummon)
        {
            player.ChangeOccur = false;
            int cnt = player.MaxHP;
            player.MaxHP = Mathf.FloorToInt(player.InitHP * (1 + player.HPRatio + GameManager.instance.PlayerStatus.hp));
            if (cnt - player.MaxHP != 0)
            {
                player.CurHP += player.MaxHP - cnt;
                HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
                if (!IsPlayer) player.MyBatch.HPBar.fillAmount = player.CurHP / (float)player.MaxHP;
                else GameManager.instance.UM.HpChange();
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
                    if (Vector3.Distance(transform.position, TargetPos.position) <= 2f) player.IsFollow = false;
                }
                else
                {
                    var Test = GameManager.GetNearest(scanRange, 2, transform.position, targetLayer);
                    TargetPos = null;
                    foreach (var k in Test) if (k != transform) TargetPos = k;
                    if (TargetPos != null)
                    {
                        float ChangeRange = AttackRange;
                        if (TargetPos.position.y > transform.position.y)
                        {
                            float xgap = Mathf.Abs(TargetPos.position.x - transform.position.x);
                            if (xgap < 2) ChangeRange = AttackRange * 0.5f;
                            else if (xgap < 4) ChangeRange = AttackRange * 0.8f;
                        }
                        if (Vector3.Distance(transform.position, TargetPos.position) <= ChangeRange) CanMove = false;
                        player.Dir = (TargetPos.position - transform.position).normalized;
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
        }
        else if (TargetPos != null)
        {
            float ChangeRange = AttackRange;
            if (TargetPos.position.y > transform.position.y)
            {
                float xgap = Mathf.Abs(TargetPos.position.x - transform.position.x);
                if (xgap < 2) ChangeRange = AttackRange * 0.5f;
                else if (xgap < 4) ChangeRange = AttackRange * 0.8f;
            }
            if (Vector3.Distance(transform.position, TargetPos.position) > ChangeRange) CanMove = true;
        }
    }



    protected override void EndBatch()
    {
        base.EndBatch();
        StartCoroutine(SyntheEffect());
        StartCoroutine(FieldEffect());
        Norm.Play();
        foreach (var k in Synthe) k.gameObject.SetActive(true);
        if (player.WeaponLevel >= 7) { FlyOne.SetActive(true); FlyTwo.SetActive(true); }
    }

    void Emit()
    {
        if (SpecO) Spec.textureSheetAnimation.SetSprite(0, ETC[1]);
        else Spec.textureSheetAnimation.SetSprite(0, ETC[0]);
        SpecO = SpecO == false;
        Spec.Play();
    }

    Vector3 SizeSub;
    IEnumerator FieldEffect()
    {
        SizeSub = new Vector3(2f, 2f, 0);
        ForceField.localScale = new Vector3(0, 0, 1);
        AIM_Force.StartMaking();
        while (true)
        { 
            ForceField.localScale += SizeSub;
            GameManager.instance.BM.MakeBuff(new BulletInfo(0, false, 0, scalefactor: MaxScale * 0.5f, buffs: new Buff(last: 0.2f, heal: (int)((1 + GameManager.instance.PlayerStatus.attack) * HealRatio), attack: ReinforceRatio, defense: ReinforceRatio)), new Vector3(transform.position.x,transform.position.y - 0.6f), null, false);
            if (ForceField.localScale.x >= MaxScale)
            {
                yield return GameManager.DotOneSec;
                ForceField.localScale = new Vector3(0, 0, 1);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SyntheEffect()
    {
        while (true)
        {
            foreach (var k in Synthe)
            {
                int j = Mathf.FloorToInt(k.fillAmount * 10);
                if (Random.Range(0, j) < 3 && k.fillAmount < 1) k.fillAmount += 0.1f;
                else if (k.fillAmount > 0) k.fillAmount -= 0.1f;
                else k.fillAmount += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    float HealRatio = 1f;
    float ReinforceRatio = 0.1f;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: HealRatio += 0.1f; break;
            case 2: MaxScale = 24; SizeSub = new Vector3(2.4f, 2.4f); break;
            case 3: HealRatio += 0.2f; break;
            case 4: MaxScale = 30; SizeSub = new Vector3(3f, 3f); break;
            case 5: ReinforceRatio = 0.15f; break;
            case 6: FlyOne.SetActive(true); FlyTwo.SetActive(true); break;
        }
        return player.WeaponLevel;
    }

}
