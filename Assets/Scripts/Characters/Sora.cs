using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sora : PlayerSetting
{
    [SerializeField] ParticleSystem Norm;
    [SerializeField] ParticleSystem Spec;
    [SerializeField] List<Image> Synthe;
    [SerializeField] Transform ForceField;
    [SerializeField] AfterImMaker AIM_Force;

    [SerializeField] List<Sprite> ETC;


    bool SpecO = false;

    int MaxScale = 15;

    protected override void Awake()
    {
        base.Awake();
        player.SubEffects.Add(transform.GetChild(1).GetComponent<SpriteRenderer>());
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var k in Synthe) k.gameObject.SetActive(false);
    }



    protected override void EndBatch()
    {
        base.EndBatch();
        StartCoroutine(SyntheEffect());
        StartCoroutine(FieldEffect());
        Norm.Play();
        foreach (var k in Synthe) k.gameObject.SetActive(true);
    }

    void Emit()
    {
        if (SpecO) Spec.textureSheetAnimation.SetSprite(0, ETC[1]);
        else Spec.textureSheetAnimation.SetSprite(0, ETC[0]);
        SpecO = SpecO == false;
        Spec.Play();
    }

    IEnumerator FieldEffect()
    {
        Vector3 Ones = new Vector3(2f, 2f, 0);
        ForceField.localScale = new Vector3(0, 0, 1);
        AIM_Force.StartMaking();
        while (true)
        {
            ForceField.localScale += Ones;
            if (ForceField.localScale.x >= MaxScale)
            {
                yield return GameManager.DotOneSec;
                ForceField.localScale = new Vector3(0, 0, 1);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator SyntheEffect()
    {
        while (true)
        {
            foreach (var k in Synthe)
            {
                int j = Mathf.FloorToInt(k.fillAmount * 10);
                if (Random.Range(0, j) < 3 && k.fillAmount < 1) k.fillAmount += 0.1f;
                else if (k.fillAmount > 0) k.fillAmount -= 0.1f;
                else k.fillAmount += 0.1f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.BM.MakeBuff(new BulletInfo(0, false, 0, buffs: new Buff(heal: (int)((1 + GameManager.instance.PlayerStatus.attack * 0.1f)))), collision.transform.position, null, false);
        }
    }
}
