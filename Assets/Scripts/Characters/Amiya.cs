using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amiya : PlayerSetting
{
    [SerializeField] Sprite Bullet1;
    [SerializeField] Sprite Bullet2;
    [SerializeField] GameObject Weapon;
    [SerializeField] GameObject Weapon2;
    [SerializeField] GameObject Crown;
    [SerializeField] BulletLine BL;
    [SerializeField] BulletLine BLT;
    [SerializeField] List<ParticleSystem> PTS;


    Sprite BulletIm;
    BulletLine CurApplyLine;
    protected override void Awake()
    {
        base.Awake();
        player.SubEffects.Add(Weapon.GetComponent<SpriteRenderer>());
        player.SubEffects.Add(Weapon2.GetComponent<SpriteRenderer>());
        player.SubEffects.Add(Crown.GetComponent<SpriteRenderer>());

        BulletIm = Bullet1;
        CurApplyLine = BL;
    }

    private void Start()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        while (true)
        {
            int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio + player.ReinforceAmount[0]) * DamageRatio * 10);
            var Targets = GameManager.GetNearest(scanRange, ProjNum, transform.position, targetLayer);
            Transform j;
            if (Targets.Count != 0)
                for (int i = 0; i < ProjNum * ProjRatio; i++)
                {
                    Vector3 RandomSub = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f)) + transform.position;
                    j = Targets[Random.Range(0, Targets.Count)];
                    PTS[i].gameObject.transform.position = RandomSub;
                    PTS[i].Play();
                    GameManager.instance.BM.MakeBullet(new BulletInfo(Damage, false, 0), Penetrate, RandomSub,
                        (j.position - transform.position).normalized, BulletSpeed, false, BulletIm,
                        BL: CurApplyLine);
                    yield return new WaitForSeconds(0.1f);
                }
            yield return new WaitForSeconds(0.25f * (2 - GameManager.instance.PlayerStatus.attackspeed));
        }
    }

    int ProjNum = 1;
    int ProjRatio = 1;
    float DamageRatio = 2f;
    int Penetrate = 0;
    float BulletSpeed = 15;


    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: Penetrate++; break;
            case 2: DamageRatio += 0.5f; break;
            case 3: ProjNum++; PTS.Add(Instantiate(PTS[0],transform.GetChild(0))); break;
            case 4: Penetrate += 2; break;
            case 5: DamageRatio += 1f; break;
            case 6:
                ProjRatio = 3; BulletIm = Bullet2; BulletSpeed = 20; Weapon2.SetActive(true); CurApplyLine = BLT;for(int i = 0; i < 4; i++) PTS.Add(Instantiate(PTS[0], transform.GetChild(0)));
                break;
        }
        return player.WeaponLevel;
    }
}

