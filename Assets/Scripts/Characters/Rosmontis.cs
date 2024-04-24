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

    protected override void Awake()
    {
        WeaponOrigin = new Vector3[2];
        WeaponSprite = new SpriteRenderer[2];
        WeaponTrail = new TrailRenderer[2];
        
        WeaponSprite[0] = SubWeapons[0].GetComponent<SpriteRenderer>(); WeaponTrail[0] = SubWeapons[0].GetComponent<TrailRenderer>();
        WeaponSprite[1] = SubWeapons[1].GetComponent<SpriteRenderer>(); WeaponTrail[1] = SubWeapons[1].GetComponent<TrailRenderer>();
        WeaponTrail[0].enabled = WeaponTrail[1].enabled = false;
        WeaponTrail[0].startColor = WeaponTrail[1].startColor = new Color(0.35f, 0.787f, 0.63f,1);
        WeaponTrail[0].endColor = WeaponTrail[1].endColor =  new Color(0.31f, 0.62f, 0.63f, 0.1f);
        
        base.Awake();
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
            WeaponTrail[1].enabled = true;
            for (int i = 0; i < 5; i++)
            {
                SubWeapons[1].transform.Translate(Vector2.up * 2);
                yield return new WaitForSeconds(0.05f);
            }
            SubWeapons[1].SetActive(false);
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
            AttackPos = TargetPos.position;
            SubWeapons[0].transform.position = TargetPos.position + Vector3.up * 10f;
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
            AttackPos = TargetPos.position;
            SubWeapons[1].transform.position = TargetPos.position + Vector3.up * 10f;
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
    }

    protected override void AttackEnd()
    {
        if (!SubWeapons[0].activeSelf)
        {
            WeaponTrail[0].enabled = false; SubWeapons[0].SetActive(true); SubWeapons[0].transform.localPosition = Vector3.zero;
            OneLast = true;
        }
        else
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
            case 6: break;
        }
        return player.WeaponLevel;
    }
}
