using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Kazemaru : PlayerSetting
{
    [SerializeField] Sprite MeeleAttack;
    [SerializeField] Sprite bullet;
    [SerializeField] GameObject DollPref;

    GameObject Doll;

    protected override void Awake()
    {
        
        if (!IsSummon)
        {
            DamageRatio = 1f;
            SpecialRatio = 2f;
        }
        else
        {
            DamageRatio = 0.5f;
            SpecialRatio = 1f;
        }

        if (!IsSummon)
        {
            Doll = Instantiate(DollPref, transform.parent);
        }

        base.Awake();

    }

    protected override void AttackMethod()
    {
        if (Vector3.Distance(TargetPos.position, transform.position) <= 2)
        {
            GameManager.instance.BM.MakeMeele(
                new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10),false,0),0.3f,
                transform.position, -player.Dir,0,false,MeeleAttack);
        }
        if (ProjNum != 0)
        {
            Vector2 Sub = (TargetPos.position - transform.position).normalized;
            float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
            if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
            for (int i = -ProjNum; i <= ProjNum; i++)
            {
                GameManager.instance.BM.MakeBullet(
                    new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * SpecialRatio * 10),false,0), 0,
                transform.position, new Vector3(Mathf.Cos(rad + 0.1f * i), Mathf.Sin(rad + 0.1f * i), 0),
                15, false,bullet);
            }
        }


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsSummon) if(!Doll.activeSelf /*&& player.WeaponLevel > 6*/ && CanMove){ Doll.SetActive(true); Doll.transform.position = transform.position + Vector3.right; }
    }

    float DamageRatio;
    float SpecialRatio;
    int ProjNum;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: DamageRatio += 0.75f; break;
            case 3: ProjNum++; AttackRange = 5; break;
            case 4: DamageRatio += 0.5f; break;
            case 5: DamageRatio += 0.75f; break;
            case 6: player.anim.SetBool("IsSpecial",true); break;
        }
        return player.WeaponLevel;
    }

    private void OnDisable()
    {
        if(!IsSummon) Doll.SetActive(false);
    }
}
