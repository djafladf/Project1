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
            GameManager.instance.BM.MakeBuff(Sora.NormalInfo, transform.position,sp, false);
            yield return GameManager.DotOneSec;
        }
    }

}
