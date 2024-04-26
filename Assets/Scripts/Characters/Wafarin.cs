using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wafarin :PlayerSetting
{
    [SerializeField] Sprite Bullet;
    [SerializeField] BulletLine BL;
    [SerializeField] GameObject Pond;
    [SerializeField] bool Test = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void AttackMethod()
    {
        if (TargetPos != null)
        {
            Vector2 Sub = (TargetPos.position - transform.position).normalized;
            float rad = Vector2.Angle(Vector2.right, Sub) * Mathf.Deg2Rad;
            if (Sub.y < 0) rad = Mathf.PI * 2 - rad;
            for (int i = -ProjNum+1; i <= ProjNum-1; i++)
            {
                GameManager.instance.BM.MakeBullet(
                    new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack + player.AttackRatio) * DamageRatio * 10),false,0),
                    Penetrate, transform.position, new Vector3(Mathf.Cos(rad + 0.25f * i), Mathf.Sin(rad + 0.25f * i)),10, false,Bullet,BL:BL);
            }
            if (player.WeaponLevel == 7 && !Pond.activeSelf)
            {
                Vector3 cnt = TargetPos.position; cnt.z = 2; Pond.transform.position = cnt;
                Pond.SetActive(true);
            }
        }
    }
    float DamageRatio = 3f;
    int Penetrate = 0;
    int ProjNum = 1;

    protected override void OnEnable()
    {
        base.OnEnable();
        Pond.SetActive(false);
    }


    protected override int WeaponLevelUp()
    {
        if (player.WeaponLevel <= 6)
        {
            switch (player.WeaponLevel++)
            {
                case 1: Penetrate++; break;
                case 2: DamageRatio += 0.5f; break;
                case 3: ProjNum += 1; break;
                case 4: Penetrate += 2; break;
                case 5: DamageRatio += 0.75f; break;
                case 6: break;
            }
        }
        return player.WeaponLevel;
    }
}
