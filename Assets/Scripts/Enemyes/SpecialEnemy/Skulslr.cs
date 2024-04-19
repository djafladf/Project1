using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Skulslr : Enemy
{

    [SerializeField] Sprite Boom;
    [SerializeField] Sprite Bullet;
    [SerializeField] ParticleSystem pt;
    [SerializeField] GameObject BPart;
    TrailRenderer BLine;
    ParticleSystem.Particle[] particles;

    protected override void Awake()
    {
        base.Awake();
        particles = new ParticleSystem.Particle[pt.main.maxParticles];
        BLine = BPart.GetComponent<TrailRenderer>();
        StartCoroutine(Line1());
    }

    private void Update()
    {
        if (pt.gameObject.activeSelf)
        {
            int numParticlesAlive = pt.GetParticles(particles);
            for (int i = 0; i < numParticlesAlive; i++)
            {
                float angle = Mathf.Atan2(particles[i].velocity.y, particles[i].velocity.x) * Mathf.Rad2Deg;
                particles[i].rotation = -angle;
            }

            pt.SetParticles(particles, numParticlesAlive);
        }
    }

    protected override void FixedUpdate()
    {
        if (!IsLive) return;

        if (OnIce) return;
        if (MoveAble && !anim.GetBool("IsAttack"))
        {
            Vector2 Dir = (GameManager.instance.player.Self.position - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;

            if(!OnHit)rigid.MovePosition(rigid.position + Dir * speed * Time.fixedDeltaTime);
        }
        if (BeginAttack && !anim.GetBool("IsAttack"))
        {
            AttackPos = Target.position;
            MoveAble = false;
            anim.SetBool("IsRange",Vector2.Distance(AttackPos, transform.position) >= 3f);
            anim.SetBool("IsAttack", true);
        }
    }

    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeBoom(10, 25, 0, transform.position, (AttackPos - transform.position).normalized, 10, Bullet, Boom, true);
    }

    protected override void AttackEnd()
    {
        base.AttackEnd();
        if (BeginAttack)
        {
            AttackPos = Target.position;
            anim.SetBool("IsRange", Vector2.Distance(AttackPos, transform.position) >= 3f);
        }
    }

    protected override void Dead()
    {
        gameObject.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            GameManager.instance.IM.MakeItem(transform.position + 
                new Vector3(Random.Range(-10, 10) * 0.3f, Random.Range(-10, 10) * 0.3f, 0),true);
        }
    }

    
    protected override void OnEnable()
    {
        base.OnEnable(); GameManager.instance.UM.BossName.text = "½ºÄÃ½´·¹´õ";
    }

    bool jj = false;
    protected override void HPChange()
    {
        GameManager.instance.UM.BossHP.fillAmount = HP / (float)MaxHP;
        if(!jj && HP <= Mathf.FloorToInt(MaxHP * 0.5f))
        {
            jj = true; pt.gameObject.SetActive(true);
            float j = (float)Damage / MaxDamage;
            MaxDamage = 40; Damage = Mathf.FloorToInt(MaxDamage * j);
            j = (float)Defense / MaxDefense;
            MaxDefense = 50; Defense = Mathf.FloorToInt(MaxDefense * j);
            BPart.SetActive(true); StartCoroutine(Line1());
        }
    }


    IEnumerator Line1()
    {
        BPart.transform.localPosition = new Vector3(2, -2, 0); BLine.Clear();
        float Theta1 = 315 * Mathf.Deg2Rad;
        float Theta2 = 135 * Mathf.Deg2Rad;
        float Theta = (Theta2 - Theta1) * 0.2f;
        float cnt = 2;
        for(int i = 1; i < 6; i++)
        {
            float NewTheta = Theta1 + Theta * i;
            BPart.transform.localPosition = new Vector3(Mathf.Cos(NewTheta) * cnt,Mathf.Sin(NewTheta) * cnt,0);
            yield return new WaitForSeconds(0.1f);
        }

        yield return GameManager.DotOneSec;
        StartCoroutine(Line2());
    }

    IEnumerator Line2()
    {
        BPart.transform.localPosition = new Vector3(-2, -2, 0); BLine.Clear();
        
        float Theta1 = 225 * Mathf.Deg2Rad;
        float Theta2 = 45 * Mathf.Deg2Rad;
        float Theta = (Theta2 - Theta1) * 0.2f;
        float cnt = Mathf.Sqrt(6);
        for (int i = 1; i < 6; i++)
        {
            float NewTheta = Theta1 - Theta * i;
            BPart.transform.localPosition = new Vector3(Mathf.Cos(NewTheta) * cnt, Mathf.Sin(NewTheta) * cnt, 0);
            yield return new WaitForSeconds(0.1f);
        }

        yield return GameManager.DotOneSec;
        StartCoroutine(Line1());

    }
}
