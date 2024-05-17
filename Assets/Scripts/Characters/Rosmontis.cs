using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Rosmontis : PlayerSetting
{
    [SerializeField] GameObject[] SubWeapons;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] BulletLine BL;
    [SerializeField] GameObject FloatWeapon;



    Vector3 VectorSub = new Vector3(-2, 15,0);

    protected override void Awake()
    {
        base.Awake();
        
    }

    IEnumerator Attacks()
    {
        Vector3 DirSub = -VectorSub.normalized;
        while (true)
        {
            var Targets = GameManager.GetNearest(scanRange, ProjNum, transform.position, targetLayer);
            if (Targets.Count != 0)
            {
                
                Transform j;
                for (int i = 0; i < ProjNum; i++)
                {
                    j = Targets[Random.Range(0, Targets.Count)];


                    GameManager.instance.BM.MakeEffect(0.4f, j.position + VectorSub, DirSub, 25, Sprites[0], BL: BL);
                    StartCoroutine(LateDamage(0.5f, j.position));
                    yield return new WaitForSeconds(0.1f);
                }
            }
            yield return GameManager.OneSec;
        }
    }

    IEnumerator LateDamage(float Time,Vector3 pos)
    {
        int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);
        yield return new WaitForSeconds(Time);
        GameManager.instance.BM.MakeMeele(
            new BulletInfo(Damage, false, 0, debuffs: new DeBuff(last: 5, defense: defenseRatio)), 0.4f,
                pos, Vector3.zero, 0, false, Sprites[2]);
    }

    protected override void EndBatch()
    {
        CanMove = true;
        StartCoroutine(Attacks());
        if (player.WeaponLevel >= 7) FloatWeapon.SetActive(true);
        FloatWeapon.transform.position = Vector3.zero;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        FloatWeapon.SetActive(false); 
    }

    float DamageRatio = 3f;
    float defenseRatio = 0.3f;

    int ProjNum = 1;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: ProjNum++; break;
            case 2: DamageRatio += 0.75f; break;
            case 3: defenseRatio += 0.2f; break;
            case 4: ProjNum++; break;
            case 5: DamageRatio += 1.25f; break;
            case 6: FloatWeapon.SetActive(true); break;
        }
        return player.WeaponLevel;
    }
}
