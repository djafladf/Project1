using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutter : PlayerSetting
{
    [SerializeField] Sprite NormalAttack;
    [SerializeField] Sprite Bullet;

    protected override void Awake()
    {
        base.Awake();
        AttackInf.AttackSpeed = 2f;
        AttackInf.FirstDelay = 1;
        AttackInf.LastDelay = 0.5f;
        player.CurHP = player.MaxHP = 50;
        player.CurSP = player.MaxSP = 10;
    }

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * DamageRatio), 0, 0.3f,
                transform.position, -player.Dir,
                0, NormalAttack, true, false);
        }
            
    }
    float DamageRatio = 4f;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio+=0.5f; break;
            case 2: DamageRatio+=0.5f; break;
            case 3: DamageRatio+=0.5f; break;
            case 4: DamageRatio+=0.5f; break;
            case 5: DamageRatio+=0.5f; break;
            case 6: StartCoroutine(Special()); break;
        }
        return player.WeaponLevel;
    }

    IEnumerator Special()
    {
        CanMove = false;
        player.anim.SetBool("IsWalk", true);
        player.anim.SetBool("IsAttack", false);
        for (int i = 0; i < 1000; i++)
        {
            transform.Rotate(Vector3.up * 7.2f);
            float rad = Random.Range(-3.14f, 3.14f);
            GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * DamageRatio), 0, 1,
                transform.position, new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0),
                15, Bullet, false, false);
            yield return new WaitForSeconds(0.01f);
        }
        CanMove = true;
        yield return new WaitForSeconds(7);
        StartCoroutine(Special());
    }
}

