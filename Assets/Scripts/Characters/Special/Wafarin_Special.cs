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
    [SerializeField] GameObject Particle;
    [SerializeField] Player Wafarin;

    Dictionary<Transform, ParticleSystem> Particles;
    BoxCollider2D Coll;

    private void Awake()
    {
        Particles = new Dictionary<Transform, ParticleSystem>();
        Particles[transform] = Particle.GetComponent<ParticleSystem>();
        Coll = GetComponent<BoxCollider2D>();

        foreach(var k in GameManager.instance.Prefs)
        {
            if (k != gameObject)
            {
                var j = Instantiate(Particle, transform).GetComponent<ParticleSystem>();
                j.transform.parent = k.transform; j.transform.localPosition = Vector3.zero;
                j.transform.localScale = Vector3.one;
                j.Stop();
                Particles[k.transform] = j;
            }
        }       
    }

    BulletInfo BI;
    BulletInfo Buff;

    private void Start()
    {
        BI = new BulletInfo(0, false, 0, dealFrom: Wafarin.name[0] - '0');
        Buff = new BulletInfo(0, false, 0,scalefactor:0.1f,buffs:new Buff(heal:0),dealFrom:BI.DealFrom);
    }

    private void OnEnable()
    {
        StartCoroutine(Attack());
        foreach(var k in Particles.Values) k.Stop();
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
        {
            Buff.Buffs.Heal = Mathf.FloorToInt((1 + GameManager.instance.PlayerStatus.attack * 0.1f));
            Particles[collision.transform].Play(); GameManager.instance.BM.MakeBuff(Buff, collision.transform.position, null, false);
        }
        if (collision.CompareTag("Enemy"))
        {
            Transform cnt = collision.transform;
            BI.Damage = (int)((1 + GameManager.instance.PlayerStatus.attack + Wafarin.AttackRatio + Wafarin.ReinforceAmount[0]) * 30);
            GameManager.instance.BM.MakeMeele(BI, 0.6f, cnt.position, Vector3.zero, 0, false, Bullet);
        }
    }
}
