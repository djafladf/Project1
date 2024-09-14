using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sora_Special : MonoBehaviour
{
    [SerializeField] Sora Sora;
    [SerializeField] Sprite sp;

    private void OnEnable()
    {
        StartCoroutine(MakeBuff());
    }

    IEnumerator MakeBuff()
    {
        while (true)
        {
            GameManager.instance.BM.MakeBuff(new BulletInfo(0, false, 0, buffs: Sora.CurBuff, debuffs: Sora.CurDeBuff), transform.position,sp, false);
            yield return GameManager.DotOneSec;
        }
    }

}
