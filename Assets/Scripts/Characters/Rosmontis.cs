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
        WeaponOrigin[0] = SubWeapons[0].transform.localPosition;
        WeaponOrigin[1] = SubWeapons[1].transform.localPosition;
        
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
        SubWeapons[0].transform.localPosition = WeaponOrigin[0]; WeaponSprite[0].flipX = false;
        SubWeapons[1].transform.localPosition = WeaponOrigin[1]; WeaponSprite[1].flipX = false;
        SubWeapons[0].SetActive(true); SubWeapons[1].SetActive(true);
    }

    int OneCount = 20;
    int OneSub = 1;
    int TwoCount = 20;
    int TwoSub = 1;

    bool OneUsed = false;
    bool TwoUsed = false;

    bool OneLast = false;

    protected override void WeaponAnim()
    {
        if (!OneUsed)
        {
            if (OneCount == 20) OneSub = -1;
            else if (OneCount == 0) OneSub = 1;
            OneCount += OneSub;

            if (player.Dir.x > 0 && !WeaponSprite[0].flipX)
            {
                WeaponSprite[0].flipX = true;
                SubWeapons[0].transform.localPosition *= Vector2.left;
                WeaponOrigin[0].x *= -1;
            }
            else if (player.Dir.x < 0 && WeaponSprite[0].flipX) {WeaponSprite[0].flipX = false;SubWeapons[0].transform.localPosition *= Vector2.left; WeaponOrigin[0].x *= -1; }
            
            if (OneCount % 10 == 0)  SubWeapons[0].transform.Translate(Vector2.up * Time.fixedDeltaTime * OneSub * 2);
        }

        if (!TwoUsed)
        {
            if (TwoCount == 20) TwoSub = -1;
            else if (TwoCount == 0) TwoSub = 1;
            TwoCount += TwoSub;

            if (player.Dir.x > 0 && !WeaponSprite[1].flipX){ WeaponSprite[1].flipX = true; SubWeapons[1].transform.localPosition *= Vector2.left; }
            else if (player.Dir.x < 0 && WeaponSprite[1].flipX) { WeaponSprite[1].flipX = false; SubWeapons[1].transform.localPosition *= Vector2.left; }

            if (TwoCount % 10 == 0) SubWeapons[1].transform.Translate(Vector2.up * Time.fixedDeltaTime * TwoSub * 2);
        }
    }

    void Attack1()
    {
        StartCoroutine(AttackSub());
    }

    IEnumerator AttackSub()
    {
        if (!OneLast)
        {
            WeaponSprite[0].sprite = Sprites[0];
            OneUsed = true;
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
            TwoUsed = true;
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

        GameManager.instance.BM.MakeMeele(Damage, 0,0.3f,
                    AttackPos, Vector3.zero, 0, Sprites[4], false, new DeBuff(5,defense:defenseRatio));
    }

    protected override void AttackEnd()
    {
        if (!OneLast)
        {
            WeaponTrail[0].enabled = false;
            SubWeapons[0].SetActive(true);
            
            if (player.sprite.flipX) WeaponSprite[0].transform.localPosition = WeaponOrigin[0] * Vector2.left;
            else SubWeapons[0].transform.localPosition = WeaponOrigin[0];
            WeaponSprite[0].flipX = player.sprite.flipX;
            OneCount = 20; OneSub = 1; OneLast = true; OneUsed = false;
        }
        else
        {
            WeaponTrail[1].enabled = false;
            SubWeapons[1].SetActive(true);
            if (player.sprite.flipX) WeaponSprite[1].transform.localPosition = WeaponOrigin[01] * Vector2.left;
            else SubWeapons[1].transform.localPosition = WeaponOrigin[1];
            WeaponSprite[1].flipX = player.sprite.flipX;
            TwoCount = 20; TwoSub = 1; OneLast = false; TwoUsed = false;
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
