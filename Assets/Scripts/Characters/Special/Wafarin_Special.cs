using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Wafarin_Special : MonoBehaviour
{
    [SerializeField] Sprite Bullet;
    WaitForSeconds ZeroDotFive = new WaitForSeconds(0.5f);
    [SerializeField] LayerMask[] Layers;
    [SerializeField] Player Wafarin;
    BoxCollider2D Coll;

    private void Awake()
    {
        Coll = GetComponent<BoxCollider2D>();
    }

    BulletInfo BI;
    BulletInfo Buff;

    private void Start()
    {
        BI = new BulletInfo(0, false, 0, dealFrom: Wafarin.Id);
        Buff = new BulletInfo(0, false, 0,scalefactor:0.1f,buffs:new Buff(attack:0.2f),dealFrom:BI.DealFrom);
    }

    private void OnEnable()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Coll.enabled = true;
        yield return ZeroDotFive;
        for(int i = 0; i < 10; i++)
        {
            Coll.enabled = false;
            yield return ZeroDotFive;
            Coll.enabled = true;
            yield return ZeroDotFive;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Player_Hide"))
            GameManager.instance.BM.MakeBuff(Buff, collision.transform.position, null, false);
        if (collision.CompareTag("Enemy"))
        {
            Transform cnt = collision.transform;
            BI.Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + Wafarin.AttackRatio + Wafarin.ReinforceAmount[0]) * 30);
            GameManager.instance.BM.MakeMeele(BI, 0.6f, cnt.position, Vector3.zero, 0, false, Bullet);
        }
    }
}
