using System;
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
    [HideInInspector] public Action Respawn;

    protected override void Awake()
    {
        

        if (!IsSummon)
        {
            Doll = Instantiate(DollPref, transform.parent);
            Doll.GetComponent<Kazemaru>().Respawn = RespawnAct;
            DamageRatio = 1f;
            SpecialRatio = 2f;
        }
        else
        {
            DamageRatio = 3.5f;
            SpecialRatio = 2f;
            ProjNum = 1;
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

    void RespawnAct()
    {
        StartCoroutine(RespawnKaze());
    }

    IEnumerator RespawnKaze()
    {
        yield return new WaitForSeconds(30);
        Doll.SetActive(true); Doll.transform.position = transform.position + Vector3.right;
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
            case 6: if (gameObject.activeSelf) { Doll.SetActive(true); Doll.transform.position = transform.position + Vector3.right; } player.anim.SetBool("IsSpecial",true); break;
        }
        return player.WeaponLevel;
    }


    protected override void EndBatch()
    {
        base.EndBatch();
        if (!IsSummon && player.WeaponLevel == 7) { Doll.SetActive(true); Doll.transform.position = transform.position + Vector3.right; }
        }

    private void OnDisable()
    {
        if (!IsSummon) Doll.SetActive(false);
        else if(Respawn != null) Respawn();
    }
}
