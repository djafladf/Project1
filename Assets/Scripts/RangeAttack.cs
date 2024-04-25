using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    List<Transform> GetNearest(int count, Vector3 Position)
    {
        targets = Physics2D.CircleCastAll(Position, scanRange, Vector2.zero, 0, targetLayer);
        List<Transform> res = new List<Transform>(count);
        List<float> diffs = new List<float>(count);
        for (int i = 0; i < count; i++) { diffs.Add(scanRange + 10); res.Add(null); }
        foreach(RaycastHit2D target in targets)
        {
            float curDiff = Vector3.Distance(Position, target.transform.position);
            for (int i = 0; i < count; i++)
            {
                if (curDiff < diffs[i])
                {
                    res.Insert(i, target.transform);
                    res.RemoveAt(count);
                    diffs.Insert(i, curDiff);
                    diffs.RemoveAt(count);
                    break;
                }
            }
        }

        return res;
    }
}
