using System.Collections;
using UnityEngine;

public class Manticore : PlayerSetting
{
    [SerializeField] GameObject HideObj;

    Coroutine Hide = null;

    [SerializeField] Sprite[] Bullets;

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            if (player.WeaponLevel < 7)
                StartCoroutine(Locker());
            else
                StartCoroutine(UpLocker());
            
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
                    0.5f, transform.position + z * i, Vector3.zero, 0, false, Bullets[0]);
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator UpLocker()
    {
        GameManager.instance.BM.MakeMeele(new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0),
                    0.5f, transform.position, Vector3.zero, 0, false, Bullets[2]);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.BM.MakeMeele(new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 15), false, 0),
                    0.5f, transform.position, Vector3.zero, 0, false, Bullets[1]);
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

    float DamageRatio = 1.5f;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: DamageRatio += 0.5f; break;
            case 3: player.AttackSpeed *= 1.2f; player.anim.SetFloat("AttackSpeed", player.AttackSpeed); break;
            case 4: DamageRatio += 0.75f; break;
            case 5: DamageRatio += 0.75f; break;
            case 6: break;
        }
        return player.WeaponLevel;
    }

}

