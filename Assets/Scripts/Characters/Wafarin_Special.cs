using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Wafarin_Special : MonoBehaviour
{
    [SerializeField] Sprite Bullet;
    WaitForSeconds ZeroDotFive = new WaitForSeconds(1f);
    [SerializeField] LayerMask[] Layers;
    [SerializeField] GameObject Particle;
    List<ParticleSystem> Particles;

    private void Awake()
    {
        Particles = new List<ParticleSystem>() { Particle.GetComponent<ParticleSystem>()};

        int i = 0;
        foreach(var k in GameManager.instance.Prefs)
        {
            if (i != 0) Particles.Add(Instantiate(Particle, transform).GetComponent<ParticleSystem>());
            Particles[i].transform.parent = k.transform; Particles[i].transform.localPosition = Vector3.zero;
            Particles[i].transform.localScale = Vector3.one;
            Particles[i].Stop();
            i++;
        }       
    }

    private void OnEnable()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack()
    {
        
        yield return ZeroDotFive;
        for(int i = 0; i < 20; i++)
        {
            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 5f, Vector2.zero, 0, Layers[0]);
            foreach(RaycastHit2D t in targets)
            {
                Transform cnt = t.transform;
                GameManager.instance.BM.MakeMeele(
                    new BulletInfo((int)((1 + GameManager.instance.PlayerStatus.attack) * 45f),false,0),0.3f,
                    cnt.position,Vector3.zero,0,false,Bullet);
            }
            for(int x = 0; x < Particles.Count; x++)
            {
                Vector3 CntPos = GameManager.instance.Prefs[x].transform.position;
                if (Vector2.Distance(transform.position, GameManager.instance.Prefs[x].transform.position) <= 5f)
                {
                    Particles[x].Play(); GameManager.instance.BM.MakeBuff(new BulletInfo(0, false, 0, buffs: new Buff(heal: (int)((1 + GameManager.instance.PlayerStatus.attack * 0.1f)))), CntPos, null, false);
                }
                else Particles[x].Stop();
            }
            yield return ZeroDotFive;
        }
        foreach (var k in Particles) k.Stop();
        gameObject.SetActive(false);
    }
}
