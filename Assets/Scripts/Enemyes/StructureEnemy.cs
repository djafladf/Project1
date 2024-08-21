using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureEnemy : MonoBehaviour
{
    [SerializeField] BulletInfo BI;
    [SerializeField] bool IsArrange;
    [SerializeField] float bullSpeed;
    [SerializeField] float AttackCool;
    [SerializeField] Sprite bull;
    [SerializeField] bool HasDebuff;
    [SerializeField] DeBuff deBuff;
    public Vector3 Dir;

    private void OnEnable()
    {
        if (!HasDebuff) deBuff = null;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        WaitForSeconds cool = new WaitForSeconds(AttackCool);
        while (true)
        {
            yield return cool;
            if (IsArrange) GameManager.instance.BM.MakeBullet(BI, 0, transform.position, Dir, bullSpeed, true, bull, deBuff);
            else GameManager.instance.BM.MakeMeele(BI, 0.3f, transform.position, Dir, bullSpeed, true, bull, null, deBuff);
        }
    }

}
