using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
    }

    private void Start()
    {
        /*GameManager.instance.ES.ExternalSpawnCall(2, -1, 10f);
        GameManager.instance.ES.ExternalSpawnCall(4, -1, 10f);
        GameManager.instance.ES.ExternalSpawnCall(3, -1, 10f);
        GameManager.instance.ES.ExternalSpawnCall(5, -1, 10f);
        GameManager.instance.ES.ExternalSpawnCall(6, -1, 10f);*/
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
        if (IsSpecial)
        {
            MoveAble = false; anim.SetTrigger("Special"); StartCoroutine(Special1());
        }
        if (BeginAttack && !anim.GetBool("IsAttack"))
        {
            AttackPos = Target.position;
            MoveAble = false;
            anim.SetBool("IsRange", Vector2.Distance(AttackPos, transform.position) >= 3f);
            anim.SetBool("IsAttack", true);
        }
    }

    protected override void AttackMethod()
    {
        GameManager.instance.BM.MakeBoom(
            new BulletInfo(10,false,0),new BulletInfo(Damage,false,0,ignoreDefense:0.5f),
            transform.position, (AttackPos - transform.position).normalized, 20, Bullet, Boom, true);
    }

    void MeeleAttack()
    {
        GameManager.instance.BM.MakeMeele(new BulletInfo(Mathf.FloorToInt(Damage * 1.5f), false, 0), 0.3f, AttackPos, Vector3.zero, 0, true);
    }

    void SpecialAttack()
    {
        List<Vector3> Targets = new List<Vector3>();
        foreach (var k in GameManager.instance.Prefs) if (k.activeSelf) Targets.Add(k.transform.position);
        Vector3 cnt = Targets[Random.Range(0, Targets.Count)]
            + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
        GameManager.instance.BM.MakeEffect(2, transform.position + Vector3.up, Vector3.up,20, Bullet);
        GameManager.instance.BM.MakeWarning(cnt,1.5f,Boom.bounds.size * 0.8f,SpecialAttackSub);
    }

    void SpecialAttackSub(Vector3 pos)
    {    
        GameManager.instance.BM.MakeMeele(new BulletInfo(Damage,false,0,ignoreDefense:0.5f), 0.3f, pos, Vector3.zero, 0, true,Boom);
    }
    int SpecialCool = 20;
    bool IsSpecial;

    IEnumerator Special1()
    {
        IsSpecial = false;
        yield return new WaitForSeconds(SpecialCool);
        IsSpecial = true;
    }


    protected override void AttackEnd()
    {
        base.AttackEnd();
        if (BeginAttack)
        {
            AttackPos = Target.position;
            Vector2 Dir = (AttackPos - transform.position).normalized;
            if (Dir.x > 0 && !spriteRenderer.flipX) spriteRenderer.flipX = true;
            else if (Dir.x < 0 && spriteRenderer.flipX) spriteRenderer.flipX = false;
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
        base.OnEnable(); GameManager.instance.UM.BossName.text = "½ºÄÃ½´·¹´õ"; IsSpecial = true;
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
            speed *= 1.5f;
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
