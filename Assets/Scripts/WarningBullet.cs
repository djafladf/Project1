using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBullet : MonoBehaviour
{
    [SerializeField] RectTransform My;
    [SerializeField] RectTransform Area;

    Vector2 size;
    Action<Vector3> AfterAct;
    float time;

    public void Init(float time,Vector2 size, Action<Vector3> act)
    {
        this.time = time; AfterAct = act; this.size = size / (time * 10);
        My.sizeDelta = size;
        Area.sizeDelta = Vector2.zero;
        StartCoroutine(MakeEffect());
    }

    IEnumerator MakeEffect()
    {
        for(int i = 0; i < time * 10; i++)
        {
            Area.sizeDelta += size;
            yield return new WaitForSeconds(0.1f);
        }
        AfterAct(transform.position);
        gameObject.SetActive(false);
    }
}
