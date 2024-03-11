using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wafarin_Special : MonoBehaviour
{
    [SerializeField] Sprite Bullet;
    WaitForSeconds ZeroDotFive = new WaitForSeconds(1f);
    [SerializeField] LayerMask[] Layers;
    [SerializeField] GameObject Particle;
    ParticleSystem[] Particles;

    private void Awake()
    {
        Particles = new ParticleSystem[6];
        for(int i = 0;i < 6; i++)
        {
            Particles[i] = Instantiate(Particle, transform).GetComponent<ParticleSystem>();
            Particles[i].Stop();
            Particles[i].gameObject.SetActive(false);
        }
        Destroy(Particle);
    }

    private void OnEnable()
    {
        StartCoroutine(Attack());
    }

    Vector3 SubUp = new Vector3(0, 0.5f, 0);
    IEnumerator Attack()
    {
        yield return ZeroDotFive;
        for(int i = 0; i < 20; i++)
        {
            RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero, 0, Layers[0]);
            foreach(RaycastHit2D t in targets)
            {
                Transform cnt = t.transform;
                GameManager.instance.BM.MakeBullet((int)(GameManager.instance.PlayerStatus.attack * 3f),0,0.2f,
                    cnt.position, Vector3.zero,0,null,false,false);
                GameManager.instance.BM.MakeBullet(0, 0, 0.2f,
                    cnt.position, Vector3.zero, 0, Bullet, true, false);
            }
            RaycastHit2D[] targetss = Physics2D.CircleCastAll(transform.position, 2f, Vector2.zero, 0, Layers[1]);
            for (int x = 0; x < 6; x++) { Particles[x].gameObject.SetActive(false); Particles[x].Stop(); }
            for (int x = 0; x < targetss.Length; x++)
            {
                Transform cnt = targetss[x].transform;
                GameManager.instance.BM.MakeBullet(-(int)(GameManager.instance.PlayerStatus.attack * 0.2f), 0, 0.1f,
                    cnt.position, Vector3.zero, 0, null, false, true);

                Particles[x].gameObject.SetActive(true);
                Particles[x].transform.position = cnt.position;
                Particles[x].Play();
            }
            

            yield return ZeroDotFive;
        }
        gameObject.SetActive(false);
    }
}
