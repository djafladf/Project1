using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mepisto : Enemy
{
    [SerializeField] ParticleSystem VirusSystem;

    Faust faust;
    protected override void OnEnable()
    {
        base.OnEnable();
        GameManager.instance.UM.BossName.text = "메피스토";
        GameManager.instance.UM.BossHP.fillAmount = 1;
        MaxHP = Mathf.FloorToInt(MaxHP * (1 + GameManager.instance.EnemyStatus.boss * 0.05f));
        MaxDefense = Mathf.FloorToInt(MaxDefense * (1 + GameManager.instance.EnemyStatus.boss * 0.05f));
        MaxDamage = Mathf.FloorToInt(MaxDamage * (1 + GameManager.instance.EnemyStatus.boss * 0.05f));
        StartCoroutine(SkillCool());
        GameManager.instance.ES.SetCurrentSpawnPos();
    }
    
    private void Start()
    {
        transform.position = GameManager.instance.player.Self.position + new Vector3(8,0);
        GameManager.instance.ES.ExternalSpawnCall(15, -1, 8f);
        GameManager.instance.ES.ExternalSpawnCall(16, -1, 8f);
        faust = GameManager.instance.ES.TakeOffObj(18).GetComponent<Faust>() ;

        int[] Sub = { -1, 0, 1 };
        for(int i = 0; i < 9; i++)
        {
            if (i == 4) continue;
            Vector3 cnt = new Vector3(Sub[i % 3] * 3, Sub[i/3]*3) + transform.position; cnt.z = 1;
            var tmp = GameManager.instance.ES.TakeOffObj(17);
            tmp.transform.position = cnt;
        }
    }

    IEnumerator SkillCool()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            anim.SetTrigger("Skill");
        }
    }

    [SerializeField]LayerMask TargetLayer;
    public void MakeHeal()
    {
        VirusSystem.Play();
        foreach(var k in GameManager.GetNearest(50, 100, transform.position, TargetLayer))
        {
            GameManager.instance.BM.MakeEffect(0.5f, k.position, Vector3.zero, 0, Bull);
        }
        GameManager.instance.BM.MakeBuff(BI, transform.position, null, true);
    }

    protected override void HPChange()
    {
        GameManager.instance.UM.BossHP.fillAmount = HP / (float)MaxHP;
        if (HP <= Mathf.FloorToInt(MaxHP * 0.5f))
        {
            Invoke("Exit", 1);
            faust.PhazeT();
        }
    }

    void Exit()
    {
        gameObject.SetActive(false);
    }

    new void Heal(float Amount) { }

    new void FixedUpdate()
    {

    }

    protected override void OnTriggerExit2D(Collider2D collision) { }


    
}
