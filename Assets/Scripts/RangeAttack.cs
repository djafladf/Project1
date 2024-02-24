using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class RangeAttack : MonoBehaviour
{
    [SerializeField] int Num;
    [SerializeField] int Damage;
    [SerializeField] float scanRange;
    [SerializeField] Sprite Image;
    [SerializeField] LayerMask targetLayer;
    RaycastHit2D[] targets;

    Transform GetNearest()
    {
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        Transform res = null;
        float diff = scanRange + 10;
        foreach(RaycastHit2D target in targets)
        {
            float curDiff = Vector3.Distance(transform.position, target.transform.position);
            if(curDiff < diff)
            {
                diff = curDiff;
                res = target.transform;
            }
        }

        return res;
    }

    public void Fire()
    {
        Transform Target = GetNearest();
        if (Target != null)
        {
            GameManager.instance.BM.MakeBullet(transform,(Target.position - transform.position).normalized,5);
        }
    }
}
