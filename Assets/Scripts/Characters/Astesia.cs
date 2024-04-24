using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astesia : PlayerSetting
{
    [SerializeField] GameObject Weapon;
    [SerializeField] Sprite Attack1;
    [SerializeField] Sprite Attack2;
    [SerializeField] Sprite Bullet;

    [SerializeField] BulletLine BL;

    Sprite AttackIm;

    public bool IsTest = false;

    protected override void Awake()
    {
        base.Awake();
        AttackIm = Attack2;
        player.SubEffects.AddRange(Weapon.transform.GetComponentsInChildren<SpriteRenderer>());
    }

    Vector3 DropGap = new Vector3(-2, 8, 0);
    Vector3 DropDir = new Vector3(2, -8, 0).normalized;
    


    protected override void AttackMethod()
    {

        if (TargetPos != null)
        {
            
            if (IsTest)
            {
                GameManager.instance.BM.MakeMeele(
                    new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0), 0.3f,
                    transform.position, -player.Dir, 0, false,im : AttackIm);
                StartCoroutine(LateDamage(0.25f));
                GameManager.instance.BM.MakeEffect(0.25f, TargetPos.position + DropGap, DropDir, 30, Bullet, BL: BL);
            }
        }
    }

    IEnumerator LateDamage(float Time)
    {
        yield return new WaitForSeconds(Time);
        GameManager.instance.BM.MakeBullet(
            new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0),0,
                TargetPos.position, Vector3.zero, 0, false);
    }

    float DamageRatio = 1f;

    protected override void EndBatch()
    {
        base.EndBatch();
        //Weapon.SetActive(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //Weapon.SetActive(false);
    }

}
