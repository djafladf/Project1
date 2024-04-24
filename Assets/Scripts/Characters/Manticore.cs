using System.Collections;
using UnityEngine;

public class Manticore : PlayerSetting
{
    [SerializeField] GameObject HideObj;

    Coroutine Hide = null;

    [SerializeField] int type = 0;

    [SerializeField] Sprite[] Bullets;

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            if (type == 0)
                GameManager.instance.BM.MakeBullet(new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0),
                    0, transform.position, (TargetPos.position - transform.position).normalized,
            15, false, Bullets[0]);
            else if (type == 1) StartCoroutine(Locker());
            else
                GameManager.instance.BM.MakeMeele(new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0),
                    0.5f, transform.position, Vector3.zero, 0, false, Bullets[2]);
            
            if (Hide == null) Hide = StartCoroutine(HideTime());
            else { StopCoroutine(Hide); Hide = StartCoroutine(HideTime()); }
        }
    }

    IEnumerator Locker()
    {
        Vector3 z = (TargetPos.position - transform.position).normalized * 2;

        for(int i = 1; i < 6; i++)
        {
            GameManager.instance.BM.MakeMeele(new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0),
                    0.5f, transform.position + z * i, Vector3.zero, 0, false, Bullets[1]);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator HideTime()
    {
        HideObj.SetActive(false);
        tag = "Player";
        yield return new WaitForSeconds(3);
        tag = "Player_Hide";
        HideObj.SetActive(true);
        Hide = null;
    }


    protected override void EndBatch()
    {
        base.EndBatch(); HideObj.SetActive(true); tag = "Player_Hide";

    }

    protected override void OnEnable()
    {
        base.OnEnable(); HideObj.SetActive(false);
    }

    float DamageRatio = 1f;
    float SpecialRatio = 2f;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: DamageRatio += 0.75f; break;
            case 3: AttackRange = 5; break;
            case 4: DamageRatio += 0.5f; break;
            case 5: DamageRatio += 0.75f; break;
            case 6: SpecialRatio = 3f;break;
        }
        return player.WeaponLevel;
    }

}

