using System.Collections;
using UnityEngine;

public class Cutter : PlayerSetting
{
    [SerializeField] Sprite NormalAttack;
    [SerializeField] Sprite Bullet;

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {

            if (Vector3.Distance(TargetPos.position, transform.position) <= 2)
            {
                GameManager.instance.BM.MakeMeele(
                    new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10), false, 0,ignoreDefense:0.2f), 0.3f,
                    transform.position, -player.Dir, 0, false, NormalAttack);
            }
            if (ProjNum != 0)
            {
                Vector2 Sub = (TargetPos.position - transform.position).normalized;
                float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
                if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
                for (int i = -ProjNum; i <= ProjNum; i++)
                {
                    GameManager.instance.BM.MakeBullet(
                        new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * SpecialRatio * 10), false, 0, ignoreDefense: 0.2f), 0,
                    transform.position, new Vector3(Mathf.Cos(rad + 0.1f * i), Mathf.Sin(rad + 0.1f * i), 0),
                    15, false, Bullet);
                }
            }
        }
    }
    protected override void EndBatch()
    {
        base.EndBatch();
    }

    float DamageRatio = 1f;
    float SpecialRatio = 2f;
    int ProjNum = 0;
    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: DamageRatio += 0.5f; break;
            case 2: DamageRatio += 0.75f; break;
            case 3: ProjNum++; AttackRange = 5; break;
            case 4: DamageRatio += 0.5f; break;
            case 5: DamageRatio += 0.75f; break;
            case 6: SpecialRatio = 3f; ProjNum++; break;
        }
        return player.WeaponLevel;
    }

    /*IEnumerator Special()
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
    }*/
}

