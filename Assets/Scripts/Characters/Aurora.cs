using System.Collections;
using UnityEngine;

public class Aurora : PlayerSetting
{
    [SerializeField] GameObject IceField;

    bool IceOn = true;

    protected override void Awake()
    {
        base.Awake();
        player.CurHP = player.MaxHP = 500;
        player.CurSP = player.MaxSP = 10;
        player.Defense = player.MaxDefense = 25;
    }

    bool IsHeal = false;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsHeal) Heal(5);
    }

    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeMeele((int)(GameManager.instance.PlayerStatus.attack), 0, 0.2f,
                    TargetPos.position, Vector3.zero, 0, null, false,new DeBuff(ice : this.ice));
    }

    int ice = 0;

    protected override int WeaponLevelUp()
    {
        switch (player.WeaponLevel++)
        {
            case 1: player.Defense += 5; player.MaxDefense += 5; break;
            case 2: player.Defense += 5; player.MaxDefense += 5; break;
            case 3: ice = 1; break;
            case 4: player.Defense += 5; player.MaxDefense += 5; break;
            case 5: IsHeal = true; break;
            case 6: IceField.SetActive(true); IceOn = true; StartCoroutine(FieldAct()); break;
        }
        return player.WeaponLevel;
    }

    IEnumerator FieldAct()
    {
        while (true)
        {
            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero, 0, targetLayer);
            foreach (RaycastHit2D t in targets)
            {
                Transform cnt = t.transform;
                GameManager.instance.BM.MakeMeele((int)(player.Defense * 1.5f), 0, 0.2f,
                    cnt.position, Vector3.zero, 0, null, false,new DeBuff(ice : 1));
            }
            yield return GameManager.OneSec;
        }
    }

    protected override void EndBatch()
    {
        base.EndBatch();
        if (IceOn)
        {
            IceField.gameObject.SetActive(true);
            StartCoroutine(FieldAct());
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        IceField.gameObject.SetActive(false);
    }
}

