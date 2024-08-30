using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaustBow : MonoBehaviour
{
    [SerializeField] BulletInfo BI;
    [SerializeField] bool IsArrange;
    public Vector3 Dir;
    public Faust Fs;

    WaitForSeconds cool = new WaitForSeconds(5);
    Vector2 StartPos;

    private void OnEnable()
    {
        StartCoroutine(Attack());
        StartPos = transform.position + Dir + new Vector3(0, 0.3f);
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return cool;
            GameManager.instance.BM.MakeBullet(BI, 1, StartPos, Dir, 25, true, Fs.bull, null);
            GameManager.instance.BM.MakeEffect(0.3f,  StartPos, Dir, 0, Fs.ShootEffect);
        }
    }

    public void SpecialShoot()
    {
        StopAllCoroutines();
        GameManager.instance.BM.MakeBullet(BI, 100, StartPos, Dir, 75, true, Fs.Spbull);
        GameManager.instance.BM.MakeEffect(0.3f, StartPos, Dir, 0, Fs.ShootEffect);
        StartCoroutine(Attack());
    }

}
