using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    Rigidbody2D rigid;
    RectTransform Rect;
    TMP_Text text;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        text = GetComponent<TMP_Text>();
        Rect = GetComponent<RectTransform>();
    }

    Color Orange = new Color(1,0.5f,0,1);
    Vector2 UB = new Vector2(-100, 200);
    public void Init(int amount, Transform pos)
    {
        gameObject.SetActive(true); text.text = $"{amount}";
        text.fontSize = 0.5f + 0.1f * (amount / 100);
        if (amount <= 100) text.color = Color.white; else if (amount <= 250) text.color = Orange; else text.color = Color.red;
        transform.position = pos.position;
        rigid.AddForce(UB);
        StartCoroutine(RemoveDamage());
    }

    WaitForSeconds s = new WaitForSeconds(0.6f);
    IEnumerator RemoveDamage()
    {
        yield return s;
        gameObject.SetActive(false);
        rigid.velocity = Vector2.zero;
    }
}
