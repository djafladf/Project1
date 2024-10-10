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

    [SerializeField] List<AudioSource> AttackSounds;

    AudioSource source;

    Sprite BulletIm;
    BulletLine CurApplyLine;
    protected override void Awake()
    {
        base.Awake();
        source = GetComponent<AudioSource>(); AttackSounds.Add(PTS[0].GetComponent<AudioSource>());
        player.SubEffects.Add(Weapon.GetComponent<SpriteRenderer>());
        player.SubEffects.Add(Weapon2.GetComponent<SpriteRenderer>());
        player.SubEffects.Add(Crown.GetComponent<SpriteRenderer>());

        BulletIm = Bullet1;
        CurApplyLine = BL;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(Test());
#if UNITY_EDITOR
        //GameManager.instance.SetTime(3, false);
        for (int i = 0; i < 6; i++) WeaponLevelUp();
#endif
    }

    IEnumerator Test()
    {
        while (true)
        {
            if (OnIce) yield return GameManager.DotOneSec;
            NormalInfo.Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio + player.ReinforceAmount[0]) * DamageRatio * 10);
            var Targets = GameManager.GetNearest(scanRange, ProjNum, transform.position, targetLayer);
            Transform j;
            if (Targets.Count != 0)
                for (int i = 0; i < ProjNum * ProjRatio; i++)
                {
                    AttackSounds[i].Play();
                    Vector3 RandomSub = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f)) + transform.position;
                    j = Targets[Random.Range(0, Targets.Count)];
                    PTS[i].gameObject.transform.position = RandomSub;
                    PTS[i].Play();
                    GameManager.instance.BM.MakeBullet(NormalInfo, Penetrate, RandomSub,
                        (j.position - transform.position).normalized, BulletSpeed, false, BulletIm,
                        BL: CurApplyLine);
                    yield return new WaitForSeconds(0.1f);
                }
            yield return new WaitForSeconds(0.25f * (2 - GameManager.instance.PlayerStatus.attackspeed));
        }
    }

    int ProjNum = 1;
    int ProjRatio = 1;
    float DamageRatio = 3f;
    int Penetrate = 0;
    float BulletSpeed = 15;


    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: Penetrate++; break;
            case 2: DamageRatio += 0.5f; break;
            case 3: ProjNum++; PTS.Add(Instantiate(PTS[0],transform.GetChild(0))); AttackSounds.Add(PTS[1].GetComponent<AudioSource>()); break;
            case 4: Penetrate += 2; break;
            case 5: DamageRatio += 1f; break;
            case 6:
                source.Play(); BulletIm = Bullet2;
                Penetrate = 50; ProjNum++; ProjRatio = 2; BulletIm = Bullet2; BulletSpeed = 50; Weapon2.SetActive(true); CurApplyLine = BLT;for (int i = 0; i < 4; i++) { PTS.Add(Instantiate(PTS[0], transform.GetChild(0))); AttackSounds.Add(PTS[i+2].GetComponent<AudioSource>()); }
                break;
        }
        return player.WeaponLevel;
    }
}

