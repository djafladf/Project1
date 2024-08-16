using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aurora_Special : MonoBehaviour
{
    [SerializeField] Player player;
    CapsuleCollider2D CC;

    private void Awake()
    {
        CC = GetComponent<CapsuleCollider2D>();
    }
    WaitForSeconds ZeroDotFive = new WaitForSeconds(0.5f);
    IEnumerator EAE()
    {
        while (true)
        {
            CC.enabled = true;
            yield return ZeroDotFive;
            CC.enabled = false;
            transform.Translate(Vector3.back);
            yield return ZeroDotFive;
            transform.Translate(Vector3.forward);
        }
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;
        StartCoroutine(EAE());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) 
        {
            Transform cnt = collision.transform;
            GameManager.instance.BM.MakeBullet(
                new BulletInfo((int)(player.InitDefense * (1 + player.DefenseRatio + GameManager.instance.PlayerStatus.defense + player.ReinforceAmount[1])), false, 0, debuffs: new DeBuff(ice: 1),scalefactor : 0.2f),
                0, cnt.position, Vector3.zero, 0, false, null);
        }
    }
}
