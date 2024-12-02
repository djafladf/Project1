using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astesia_Special : MonoBehaviour
{
    [SerializeField] Player Astesia;
    SpriteRenderer Sprite;
    Coroutine color = null;
    Color Sub = new Color(0.75f, 0.75f, 1);


    BulletInfo BI;
    private void Awake()
    {
        Sprite = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        BI = new BulletInfo(0, false, 10, dealFrom: Astesia.Id);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (color == null) color = StartCoroutine(ColorChange());
            BI.Damage = Mathf.FloorToInt((1 + GameManager.instance.PlayerStatus.attack + Astesia.AttackRatio + Astesia.ReinforceAmount[0] + GameManager.instance.PlayerStatus.defense + Astesia.DefenseRatio + Astesia.ReinforceAmount[1]) * 15);
            GameManager.instance.BM.MakeMeele(BI, 0, collision.transform.position, Vector3.zero, 0, false);
        }
    }

    IEnumerator ColorChange()
    {
        Sprite.color = Sub;
        yield return GameManager.OneSec;
        Sprite.color = Color.white;
        color = null;
    }
}
