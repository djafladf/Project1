using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaustBow : MonoBehaviour
{
    [SerializeField] BulletInfo BI;
    [SerializeField] bool IsArrange;
    [SerializeField] Sprite bull;
    [SerializeField] Sprite shootEf;
    public Vector3 Dir;

    private void OnEnable()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        WaitForSeconds cool = new WaitForSeconds(5);
        Vector2 StartPos = transform.position + Dir + new Vector3(0, 0.3f);
        while (true)
        {
            yield return cool;
            GameManager.instance.BM.MakeBullet(BI, 1, StartPos, Dir, 15, true, bull, null);
            GameManager.instance.BM.MakeEffect(0.3f,  StartPos, Dir, 0, shootEf);
        }
    }

}
