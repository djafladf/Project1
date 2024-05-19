using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningBullet : MonoBehaviour
{
    [SerializeField] RectTransform My;
    [SerializeField] RectTransform Area;

    [SerializeField] Image P1;
    [SerializeField] Image P2;

    Vector2 size;
    Action<Vector3> AfterAct;
    float time;

    public void Init(float time,Vector2 size, Action<Vector3> act, Color S)
    {
        this.time = time; AfterAct = act; this.size = size / (time * 10);
        P1.color = S; P2.color = S;
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
