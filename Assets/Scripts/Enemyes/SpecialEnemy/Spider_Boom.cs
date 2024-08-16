using System.Collections;
using UnityEngine;

public class Spider_Boom : Enemy
{
    [SerializeField] Sprite Boom;
    protected override IEnumerator DeadLater()
    {
        GameManager.instance.BM.MakeWarning(transform.position,0.75f, Boom.bounds.size, Color.red, SpecialAttackSub);
        return base.DeadLater();
    }

    void SpecialAttackSub(Vector3 pos)
    {
        GameManager.instance.BM.MakeMeele(new BulletInfo(Mathf.FloorToInt(Damage * (1 + GameManager.instance.EnemyStatus.attack - DeBuffVar[1])), false, 0, ignoreDefense: 0.25f), 0.3f, pos, Vector3.zero, 0, true, Boom);
    }
}
