using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platinum :PlayerSetting
{
    [SerializeField] Sprite Bullet;
    [SerializeField] Sprite Bullet2;
    [SerializeField] Sprite SpecBul;
    [SerializeField] ParticleSystem OneGi;
    [SerializeField] Transform Mark;
    [SerializeField] BulletLine BL;
    [SerializeField] BulletLine BL2;

    bool MakeSpec = false;


    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            Vector2 Sub = (TargetPos.position - transform.position).normalized;
            float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
            if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
            int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);
            for (int i = -ProjNum+1; i <= ProjNum-1; i++)
            {
                GameManager.instance.BM.MakeBullet(
                    new BulletInfo(Damage,false,0,ignoreDefense : DefenseIgnore),Penetrate,
                transform.position, new Vector3(Mathf.Cos(rad + 0.25f * i), Mathf.Sin(rad + 0.25f * i)),
                15, false,Bullet,BL:BL);
            }

            MakeSpec = true;
        }
    }

    protected override void AttackEnd()
    {
        if (MakeSpec) { player.anim.SetTrigger("Spec"); Target = TargetPos; }
        else base.AttackEnd();
    }

    Transform Target;

    void ShowerSetting()
    {
        MakeSpec = true;

        if (player.sprite.flipX) GameManager.instance.BM.MakeEffect(0.3f, transform.position, new Vector3(1, 1, 0), 25, SpecBul, BL: BL2);
        else GameManager.instance.BM.MakeEffect(0.5f, transform.position, new Vector3(-1, 1, 0), 25, SpecBul, BL: BL2);
        StartCoroutine(RainSub(Target.position));
    }
    


    Vector3 OneO = new Vector3(-2, 0.84f, 0);
    Vector3 OneT = new Vector3(2, 0.84f, 0);
    void StartOnegi()
    {
        if (player.sprite.flipX) OneGi.gameObject.transform.localPosition = OneT;
        else OneGi.gameObject.transform.localPosition = OneO;

        Mark.gameObject.SetActive(true);
        if (Target.gameObject.activeSelf) { Mark.transform.parent = Target; Mark.transform.localPosition = Vector3.up * 2; }
        else Target.transform.position = Target.position + Vector3.up * 2;

        OneGi.Play();
    }

    void StopOnegi()
    {
        OneGi.Stop();
    }
    

    IEnumerator RainSub(Vector3 Target)
    {
        yield return new WaitForSeconds(0.3f);
        int Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10);
        var cnt = GameManager.GetNearest(5, 7, Target, targetLayer);
        Vector3 Gap = new Vector3(0, 15, 0);
        for (int i = 0; i < 7; i++)
        {
            Vector3 RandomSub = Target + Gap;
            if (cnt.Count != 0) RandomSub = cnt[Random.Range(0, cnt.Count)].position + Gap;
            GameManager.instance.BM.MakeMeele(new BulletInfo(Damage, false,0,ignoreDefense:DefenseIgnore),0.4f,RandomSub,Vector3.down,30,false,Bullet2,delay:0.3f);
            yield return GameManager.DotOneSec;
        }
        Mark.gameObject.SetActive(false);
        MakeSpec = false;
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        Mark.gameObject.SetActive(false);
        OneGi.Stop();
    }

    float DamageRatio = 3f;
    float DefenseIgnore = 0;
    int Penetrate = 0;
    int ProjNum = 1;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio++; break;
            case 2: Penetrate++; break;
            case 3: DamageRatio ++; break;
            case 4: ProjNum++; break;
            case 5: DamageRatio ++; break;
            case 6:
                Penetrate = 100;
                DefenseIgnore = 0.5f;
                break;
        }
        return player.WeaponLevel;
    }
}
