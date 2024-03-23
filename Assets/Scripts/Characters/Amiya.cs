using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amiya : PlayerSetting
{
    [SerializeField] Sprite Bullet1;
    [SerializeField] Sprite Bullet2;
    [SerializeField] GameObject Weapon;

    [SerializeField] GameObject Weapon2;




    Sprite BulletIm;
    RangeAttack MyAttack;
    protected override void Awake()
    {
        base.Awake();

        player.Weapon = Weapon; player.WeaponPos = player.Weapon.transform.localPosition; player.FlipWeaponPos = new Vector3(-player.WeaponPos.x, player.WeaponPos.y);

        MyAttack = GetComponent<RangeAttack>();
        player.WeaponSprite = player.Weapon.GetComponent<SpriteRenderer>();

        BulletIm = Bullet1;
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        while(true){
            int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);
            for (int i = 0; i < ProjNum * ProjRatio; i++)
            {
                Vector3 RandomSub = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f));
                MyAttack.Fire
                    (1, Damage,
                    Penetrate,
                    transform.position + RandomSub,10,
                    BulletIm,false);
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector3 RotateSub = new Vector3(30f, 60f, 90f);

    protected override void WeaponAnim()
    {
        Weapon2.transform.Rotate(RotateSub * Time.fixedDeltaTime);
    }

    int ProjNum = 1;
    int ProjRatio = 1;
    float DamageRatio = 2f;
    int Penetrate = 0;


    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: Penetrate++; break;
            case 2: DamageRatio += 0.5f; break;
            case 3: ProjNum++; break;
            case 4: Penetrate+=2; break;
            case 5: DamageRatio += 1f; break;
            case 6:
                ProjRatio = 3;
                BulletIm = Bullet2;
                Weapon2.SetActive(true);
                break;
        }
        return player.WeaponLevel;
    }
}
        
