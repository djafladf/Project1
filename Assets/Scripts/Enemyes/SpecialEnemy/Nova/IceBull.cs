using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBull : MonoBehaviour
{
    SpriteRenderer sprite;
    [HideInInspector] public FrostNova nova;


    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        transform.parent = GameManager.instance.BM.transform;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(Test());
    }

    WaitForSeconds WFS = new WaitForSeconds(0.15f);

    IEnumerator Test()
    {
        foreach(var j in nova.IceBullSprites)
        {
            sprite.sprite = j;
            yield return WFS;
        }
        yield return new WaitForSeconds(2 + Random.Range(-2, 2) * 0.1f);
        nova.ShootIceBull(transform.position);
        gameObject.SetActive(false);
    }
}
